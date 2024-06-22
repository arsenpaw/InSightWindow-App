using Android.Gms.Common.Apis;
using AXProductApp.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static AXProductApp.Data.LinkToHub;

namespace AXProductApp.Services
{
    class LoginService : ILoginService
    {
        private readonly string _Url = $"{RealeseUrl}api/UsersDb/login";

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
                        string responseBody = await responce.Content.ReadAsStringAsync();

                       
                        dynamic responseObject = JsonConvert.DeserializeObject(responseBody);

                        
                        string token = responseObject?.token;

                        await SecureStorage.SetAsync("token", token);

                        responceStr = responce.StatusCode.ToString();
                    }
                    
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await App.Current.MainPage.DisplayAlert("Oops", "Incorrect email or password", "Ok");
                }

                return responceStr;
            }
        }
    }
}
