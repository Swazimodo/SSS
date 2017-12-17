using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebHelper.Models;

namespace SSS.Utilities.Interfaces
{
    public interface IApplicationRoles
    {
        List<ApplicationRole> GetApplicationRoles();
    }
}
