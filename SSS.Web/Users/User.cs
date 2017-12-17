using System;

namespace SSS.Web.Users
{
    /// <summary>
    /// Default implementation of IUser
    /// </summary>
    public class User
    {
        public string Domain { get; set; }
        public string LoginName { get; set; }
        public string EmployeeId { get; set; }
        public Guid UserGuid { get; set; }
    }
}
