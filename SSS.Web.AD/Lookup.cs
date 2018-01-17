using System;
using System.Linq;
using System.Security.Principal;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using SSS.Utilities.Exceptions;
using SSS.Utilities.Interfaces;

namespace SSS.Web.AD
{
    public static class Lookup
    {
        static PrincipalContext _currentDomain = null;

        /// <summary>
        /// Domain context
        /// </summary>
        public static PrincipalContext CurrentDomain
        {
            get
            {
                if (_currentDomain == null)
                    _currentDomain = new PrincipalContext(ContextType.Domain, "goa.ds.gov.ab.ca");
                return _currentDomain;
            }
            set
            {
                _currentDomain = value;
            }
        }

        /// <summary>
        /// Finds all users that belong to an AD security group
        /// </summary>
        /// <param name="groupName">AD group name</param>
        /// <returns></returns>
        public static async Task<List<User>> GetADUsersByGroup(string groupName)
        {
            List<User> users = new List<User>();
            try
            {
                //remove domain
                int index = groupName.IndexOf('\\');
                if (index > 0)
                    groupName = groupName.Substring(index + 1);

                //search for group
                GroupPrincipal group = await Task.Run(() => GroupPrincipal.FindByIdentity(CurrentDomain, IdentityType.Name, groupName));

                if (group.Members.Count > 0)
                {
                    foreach (Principal principal in group.Members)
                    {
                        UserPrincipal user = principal as UserPrincipal;
                        if (user != null)
                        {
                            User u = new User();

                            // GOA\First.Last
                            string s = user.Sid.Translate(typeof(NTAccount)).ToString();
                            string[] arr = s.Split('\\');
                            if (arr.Length < 2)
                                throw new ADLookupException("Error with user name formatting");

                            u.Domain = arr[0];
                            u.LoginName = arr[1];
                            u.EmployeeId = user.EmployeeId;
                            u.UserGuid = user.Guid ?? Guid.Empty;

                            users.Add(u);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ADLookupException("Exception thrown from looking up members in group " + groupName, ex);
            }

            return users;
        }

        public static async Task<List<User>> GetADUsersByGroup(IEnumerable<string> groupNames)
        {
            List<User> users = new List<User>();
            List<Task<List<User>>> workList = new List<Task<List<User>>>();
            foreach (string groupName in groupNames)
                workList.Add(Task.Run(() => GetADUsersByGroup(groupName)));
            foreach (Task<List<User>> task in workList)
            {
                List<User> groupUsers = await task;
                users.AddRange(groupUsers);
            }
            return users;
        }

        public static async Task<List<User>> GetADUsersByGroup(string[] groupNames)
        {
            List<User> users = new List<User>();
            List<Task<List<User>>> workList = new List<Task<List<User>>>();
            foreach (string groupName in groupNames)
                workList.Add(Task.Run(() => GetADUsersByGroup(groupName)));
            foreach (Task<List<User>> task in workList)
            {
                List<User> groupUsers = await task;
                users.AddRange(groupUsers);
            }
            return users;
        }

        /// <summary>
        /// Gets user login name and domain
        /// </summary>
        /// <param name="context">current HttpContext</param>
        /// <param name="userInfo">UserInfo obj to save values to</param>
        public static void GetUserName(HttpContext context, UserInfo userInfo)
        {
            if (string.IsNullOrEmpty(context.User.Identity.Name))
                throw new AuthorizationException("Unable to get current identity name");

            string[] user = context.User.Identity.Name.Split('\\');
            if (user.Length < 2)
                throw new AuthorizationException("Error with user name formatting");

            userInfo.Domain = user[0];
            userInfo.LoginName = user[1];
        }

        /// <summary>
        /// Gets user login name and domain
        /// </summary>
        /// <param name="context">current HttpContext</param>
        public static string GetUserName(HttpContext context)
        {
            if (string.IsNullOrEmpty(context.User.Identity.Name))
                throw new AuthorizationException("Unable to get current identity name");

            string[] user = context.User.Identity.Name.Split('\\');
            if (user.Length < 2)
                throw new AuthorizationException("Error with user name formatting");

            return user[1];
        }

        ///// <summary>
        ///// Gets current user details from AD
        ///// </summary>
        ///// <returns>User information from AD</returns>
        //public static async Task GetUserInfo(HttpContext context, UserInfo userInfo)
        //{
        //    if (string.IsNullOrEmpty(context.User.Identity.Name))
        //        throw new AuthorizationException("Unable to get current identity name");

        //    string[] user = context.User.Identity.Name.Split('\\');
        //    if (user.Length < 2)
        //        throw new AuthorizationException("Error with user name formatting");

        //    userInfo.Domain = user[0];
        //    userInfo.LoginName = user[1];

        //    //does a lookup to AD and populates values from it
        //    await GetADUserDetails(userInfo);
        //}

        /// <summary>
        /// checks to see if current user is in a AD group
        /// </summary>
        /// <param name="userInfo">target user</param>
        /// <param name="groupName">group to check for</param>
        /// <returns>true if user is in group</returns>
        [Obsolete("This method does not check well know security principals carrectly (ex. This Organization)")]
        public static bool IsUserInGroup(this UserInfo userInfo, string groupName)
        {
            foreach (string userGroup in userInfo.UserGroups)
            {
                //check if user has any mantching groups with the roles groups
                if (string.Equals(userGroup, groupName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// checks to see if current user is in a AD groups
        /// </summary>
        /// <param name="userInfo">target user</param>
        /// <param name="groupName">groups to check for</param>
        /// <returns>true if user is in one of the collection of groups</returns>
        [Obsolete("This method does not check well know security principals carrectly (ex. This Organization)")]
        public static bool IsUserInGroup(this UserInfo userInfo, IEnumerable<string> groupNames)
        {
            foreach (string groupName in groupNames)
                if (!string.IsNullOrEmpty(groupName) && userInfo.IsUserInGroup(groupName))
                    return true;
            return false;
        }

        public static async Task GetUserRoles(HttpContext context, UserInfo userInfo, IApplicationRoles roles, IAuthorizationService authService)
        {
            List<ApplicationRole> appRoles = roles.GetApplicationRoles();
            List<Tuple<string, Task<AuthorizationResult>>> work = new List<Tuple<string, Task<AuthorizationResult>>>();

            //start async tasks
            foreach (ApplicationRole role in appRoles)
                work.Add(new Tuple<string, Task<AuthorizationResult>>(role.RoleName, authService.AuthorizeAsync(context.User, role.RoleName)));

            List<string> userRoles = new List<string>();
            foreach (var workItem in work)
            {
                if ((await workItem.Item2).Succeeded)
                    userRoles.Add(workItem.Item1);
            }

            userInfo.Roles = userRoles;
        }

        /// <summary>
        /// gets additional user details from AD
        /// </summary>
        /// <param name="userInfo">expects LoginName to be set</param>
        public static async Task GetADUserDetails(UserInfo userInfo)
        {
            // find the user
            UserPrincipal user = await Task.Run(() => UserPrincipal.FindByIdentity(CurrentDomain, userInfo.LoginName));

            // if user is found
            if (user == null)
                throw new ADLookupException($"AD user {userInfo.LoginName} lookup failed");

            userInfo.IsAccountLocked = user.IsAccountLockedOut();
            userInfo.IsActive = user.Enabled ?? false; //null values become false
            userInfo.FirstName = user.GivenName;
            userInfo.LastName = user.Surname;
            userInfo.KnownAs = user.DisplayName;
            userInfo.UserGuid = user.Guid ?? Guid.Empty;

            var groups = await Task.Run(() => user.GetGroups());
            foreach (Principal p in groups)
            {
                if (p is GroupPrincipal)
                    userInfo.UserGroups.Add(p.Name);
            }
        }

        /// <summary>
        /// Checks to see if user is in a particular role
        /// </summary>
        /// <param name="user">user to analyze</param>
        /// <param name="roleName">name of role to check</param>
        /// <returns>true if user is in role</returns>
        public static bool IsInRole(this UserInfo user, string roleName)
        {
            if (!string.IsNullOrEmpty(user.Roles.Find(r => string.Equals(r, roleName))))
                return true;
            return false;
        }

        /// <summary>
        /// Looks through UserInfo roles to see if the one in question has access
        /// </summary>
        /// <param name="user">user to analyze</param>
        /// <param name="role">role to check</param>
        /// <returns>true if user is in role</returns>
        public static bool IsInRole(this UserInfo user, ApplicationRole role)
        {
            return user.IsInRole(role.RoleName);
        }
    }
}
