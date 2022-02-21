using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.ViewModels.User
{
    public class ChangePasswordCommand
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
