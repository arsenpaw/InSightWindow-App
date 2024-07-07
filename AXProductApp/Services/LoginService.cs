
using AXProductApp.Interfaces;
using AXProductApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
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
        private readonly string _Url = $"{RealeseUrl}api/Auth/login";
        private IRefreshTokenService _refreshTokenService;

         public LoginService(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        public async Task<bool> TryUserAutoLoggingAsync()
        {
            var oldUserStr = await SecureStorage.GetAsync(nameof(UserDetail));

            if (!string.IsNullOrWhiteSpace(oldUserStr))
            {
                var statusCode = await _refreshTokenService.UpdateTokens();
                var user = JsonConvert.DeserializeObject<UserDetail>(await SecureStorage.GetAsync(nameof(UserDetail)));
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(user.Token) as JwtSecurityToken;
                if (jwtToken.ValidTo > DateTime.UtcNow && statusCode == HttpStatusCode.OK)
                     return true;
                else 
                     await App.Current.MainPage.DisplayAlert("Oops", "Something wrong happen while autologing", "O shit,here we go againg");
            }
            return false;
        }

        public async Task WriteTokenDataToStorage(string jwtToken, string refreshToken )
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;
            var userId = jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId).Value;
         
            var user = new UserDetail { Token = jwtToken, Id = userId, RefreshToken = refreshToken };//add role
            var userStr = JsonConvert.SerializeObject(user);
            await SecureStorage.SetAsync(nameof(UserDetail), userStr);
        }

        public async Task<bool> TryAuthenticateUserAsync(UserLoginModel userLogin)
        {
            using (var httpClient = new HttpClient())
            {
                var objectToSendStr = JsonConvert.SerializeObject(userLogin);
                try
                {
                    var responce = await httpClient.PostAsync(_Url, new StringContent(objectToSendStr, Encoding.UTF8, "application/json"));
                    if (responce.IsSuccessStatusCode)
                    {
                        string responseBody = await responce.Content.ReadAsStringAsync();
                        var token = responce.Headers.FirstOrDefault(x => x.Key == "token").Value.First();
                        var refreshToken = responce.Headers.FirstOrDefault(x => x.Key == "refresh-token").Value.First();
                        if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(token))
                            throw new Exception("One of tokens are not valid");
                        await WriteTokenDataToStorage(token, refreshToken);
                        return  true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await App.Current.MainPage.DisplayAlert("Critical", "Some trouble has happened during registration", "Ok");
                    throw new Exception("Error while AuthenticateUser");
                }
                return false;
            }
        }
    }
}
