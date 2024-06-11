using AXProductApp.Models;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AXProductApp.Data.LinkToHub;
namespace AXProductApp.Services
{
    class RegisterService : IRegisterService
    {
        private readonly string _Url = $"{RealeseUrl}api/UsersDb/create";

        public async Task<string> AuthenticateUser(UserLoginModel userLogin)
        {

            using (var httpClient = new HttpClient())
            {
                string responceStr = String.Empty;
                var objectToSendStr = JsonConvert.SerializeObject(userLogin);
                try
                {
                    var responce = await httpClient.PostAsync(_Url, new StringContent(objectToSendStr, Encoding.UTF8, "application/json"));
                    if (responce.IsSuccessStatusCode)
                    {
                        responceStr =  responce.StatusCode.ToString();
                    }
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await App.Current.MainPage.DisplayAlert("Oops", "Error ocured", "Ok");
                }

                return responceStr;
            }
        }
    }
}
