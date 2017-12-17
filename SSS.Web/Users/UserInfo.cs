using System.Collections.Generic;
using SSS.Web.Security;

namespace SSS.Web.Users
{
    /// <summary>
    /// Detailed user information with custom details
    /// </summary>
    /// <typeparam name="T">Additional application specific user information</typeparam>
    public class UserInfo<T> : UserInfo
    {
        /// <summary>
        /// This allows you to extend this object with additional user information
        /// </summary>
        public T Details { get; set; }
    }

    /// <summary>
    /// Default implementation of IUserInfo
    /// </summary>
    public class UserInfo : User
    {
        public int UserId { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string KnownAs { get; set; }
        public bool IsActive { get; set; }
        public bool IsAccountLocked { get; set; }

        List<string> _groups = null;
        public List<string> UserGroups
        {
            get
            {
                if (_groups == null)
                    _groups = new List<string>();
                return _groups;
            }
            set
            {
                _groups = value;
            }
        }

        List<ApplicationRole> _roles = new List<ApplicationRole>();

        /// <summary>
        /// list of only roles user has access to
        /// </summary>
        public List<ApplicationRole> Roles
        {
            get
            {
                if (_roles == null)
                    _roles = new List<ApplicationRole>();
                return _roles;
            }
            set
            {
                _roles = value;
            }
        }

        //List<string> _roles = new List<string>();

        ///// <summary>
        ///// list of only roles user has access to
        ///// </summary>
        //public List<string> Roles
        //{
        //    get
        //    {
        //        if (_roles == null)
        //            _roles = new List<string>();
        //        return _roles;
        //    }
        //    set
        //    {
        //        _roles = value;
        //    }
        //}
    }
}
