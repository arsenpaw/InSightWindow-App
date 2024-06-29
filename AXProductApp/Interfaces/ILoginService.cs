using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AXProductApp.Models;

namespace AXProductApp.Interfaces
{
    interface ILoginService
    {
        public Task<string> AuthenticateUser(UserLoginModel userLogin);

        public Task<bool> TryUserAutoLoggingAsync();
        
    }
}
