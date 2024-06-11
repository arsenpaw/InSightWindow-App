using AXProductApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp.Services
{
    interface ILoginService
    {
        public Task<string> AuthenticateUser(UserLoginModel userLogin); 
    }
}
