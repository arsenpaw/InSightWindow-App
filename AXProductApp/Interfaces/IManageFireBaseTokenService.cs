using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp.Interfaces
{
    public interface IManageFireBaseTokenService
    {
 

        public  Task CreateToken();



        public  Task SendTokenToServer(string token);
    }
}
