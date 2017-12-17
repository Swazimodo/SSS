using System.Collections.Generic;
using System.Reflection;

using SSS.Utilities.Interfaces;

namespace SSS.Web.Security
{
    /// <summary>
    /// This class should be inherited. Add properties for different application roles that have an enumerable list of AD groups.
    /// This allows for strongly typed role names but still processing them dynamically as a list.
    /// </summary>
    public abstract class ApplicationRolesBase// : IApplicationRoles
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
