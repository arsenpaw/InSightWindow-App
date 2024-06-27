using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AXProductApp.Data.LinkToHub;
using AXProductApp.Interfaces;
using Newtonsoft.Json;
using System.Diagnostics;
using AXProductApp.Models;
using System.Net.Http.Headers;
using System.Net;
namespace AXProductApp.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly string _Url = $"{RealeseUrl}api/UsersDb/refresh-tokens";

        public async Task<HttpStatusCode> UpdateTokens()//new tokens will be writen to secure storage
        {
            var oldUserstr = await SecureStorage.GetAsync(nameof(UserDetail));
            var oldUser = JsonConvert.DeserializeObject<UserDetail>(oldUserstr);
           
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("refresh-token", oldUser.RefreshToken);
                    
                    var response = await httpClient.PostAsync(_Url, null);

                    if (response.IsSuccessStatusCode)
                    {
                        oldUser.Token = response.Headers.First(x => x.Key == "token").Value.First();
                        oldUser.RefreshToken = response.Headers.First(x => x.Key == "refresh-token").Value.First();
                        await SecureStorage.SetAsync(nameof(UserDetail), JsonConvert.SerializeObject(oldUser));
                    }
                   return response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General error: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Oops", "An error occurred while handling responce", "Ok");
                throw new Exception("Some mistake happen while hadndling responce");
            }

        }

    }
}
