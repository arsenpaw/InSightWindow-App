using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp.Interfaces
{
    interface IRegisterService
    {
        public Task<string> AuthenticateUser(UserLoginModel userLogin);
    }
}
