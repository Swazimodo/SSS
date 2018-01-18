using System.Collections.Generic;
using System.Reflection;

using SSS.Utilities;
using SSS.Web.Security;

namespace SSS.Web.Configuration
{
    /// <summary>
    /// This class should be inherited. Add properties for different application roles that have an enumerable list of user groups.
    /// This allows for strongly typed role names but still processing them dynamically as a list.
    /// </summary>
    public abstract class ApplicationRolesBase
    {
        static List<ApplicationRole> _applicationRoles = null;

        /// <summary>
        /// Use reflection to find all application roles from the properties
        /// </summary>
        /// <returns>list of all application roles</returns>
        public virtual List<ApplicationRole> GetApplicationRoles()
        {
            List<ApplicationRole> roles = new List<ApplicationRole>();

            //find all properties that have a collection of strings and return them
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                //check if this property implements IEnumberable<string>
                if (typeof(IEnumerable<string>).IsAssignableFrom(property.PropertyType))
                //if (groups != null && groups.Count() != 0)
                {
                    IEnumerable<string> groups = property.GetValue(this) as IEnumerable<string>;

                    //If no groups are specified use a default group that will never have user accounts
                    //this is because if no groups are specified the application role will not be created
                    if (groups == null)
                        groups = new string[] { "Domain Controllers" };

                    ApplicationRole role = new ApplicationRole()
                    {
                        RoleName = property.Name,
                        Groups = groups
                    };
                    roles.Add(role);
                }
            }

            return roles;
        }
    }
}
