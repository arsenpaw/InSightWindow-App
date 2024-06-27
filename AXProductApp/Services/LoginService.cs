
using AXProductApp.Interfaces;
using AXProductApp.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static AXProductApp.Data.LinkToHub;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AXProductApp.Services
{
    class LoginService : ILoginService
    {
        private readonly string _Url = $"{RealeseUrl}api/UsersDb/login";

       

        public async Task WriteTokenDataToStorage(string jwtToken, string refreshToken )
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;
            var userId = jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
         
            var user = new UserDetail { Token = jwtToken, Id = userId, RefreshToken = refreshToken };//add role
            var userStr = JsonConvert.SerializeObject(user);
            await SecureStorage.SetAsync(nameof(UserDetail), userStr);
        }

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
                        var token = responce.Headers.FirstOrDefault(x => x.Key == "Token").Value.First();
                        var refreshToken = responce.Headers.FirstOrDefault(x => x.Key == "Refresh-Token").Value.First();
                        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(token))
                            throw new Exception("One of tokens are not valid");
                        await WriteTokenDataToStorage(token, refreshToken);
                        responceStr = responce.StatusCode.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await App.Current.MainPage.DisplayAlert("Critical", "Some trouble has happened during registration", "Ok");
                }

                return responceStr;
            }
        }
    }
}
