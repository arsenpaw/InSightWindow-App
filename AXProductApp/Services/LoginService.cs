using AXProductApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static AXProductApp.Data.LinkToHub;

namespace AXProductApp.Services
{
    class LoginService : ILoginService
    {
        private readonly string _Url = $"{RealeseUrl}/api/UsersDb";

        public async Task<string> AuthenticateUser(UserLoginModel userLogin)
        {
            using (var httpClient = new HttpClient()) 
            {
                string responceStr;
                var objectToSendStr = JsonConvert.SerializeObject(userLogin);
                var responce = await httpClient.PostAsync(_Url,new StringContent(objectToSendStr,Encoding.UTF8,"Application/json"));
                if (responce.IsSuccessStatusCode)
                {
                    responceStr = await responce.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
