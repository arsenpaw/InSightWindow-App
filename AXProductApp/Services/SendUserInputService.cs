using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXProductApp.Data
{
    public class SendUserInputService
    {
        UserInputStatus userInputStatus = new UserInputStatus(); 
        public async Task SendOpenInfo(bool isOpened)
        {
            userInputStatus.IsOpen = isOpened;
            Debug.WriteLine($"Is open: {userInputStatus.IsOpen}");
        }

        public async Task SendProtectedInfo(bool isProtected)
        {
            userInputStatus.isProtected = isProtected;
            Debug.WriteLine($"Is protected: {userInputStatus.isProtected}");
        }
    }
}
