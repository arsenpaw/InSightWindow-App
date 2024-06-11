using AXProductApp.Models;
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
                        responceStr = await responce.Content.ReadAsStringAsync();
                    }
                    return responceStr;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message); 
                    throw;
                }
               
                
            }
        }
    }
}
