using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AXProductApp.Interfaces;

namespace AXProductApp.Services
{
    public class MainMenuService : IMainMenu
    {
        private string _token = String.Empty;

        public async Task OnAppUpdate()
        {
            await OnInitializedAsync();
        }

        public async Task OnInitializedAsync()
        {
            _token = await SecureStorage.GetAsync("token");
            Debug.Write(_token);
            if (_token == String.Empty) 
            {
                throw new Exception("Token value is empty");
            }
          //request here
        }
    }
}
