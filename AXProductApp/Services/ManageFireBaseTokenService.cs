using AXProductApp.Interfaces;
using Plugin.Firebase.CloudMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Plugin.Firebase.CloudMessaging;
using System.Diagnostics;
using AXProductApp.Models.Dto;
using AXProductApp.Models;
using Java.Net;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using static AXProductApp.Data.LinkToHub;
using InSightWindowAPI.Models;
using System.Web;
using Kotlin.Jvm.Internal;
namespace AXProductApp.Services
{
    public class ManageFireBaseTokenService : IManageFireBaseTokenService
    {
        private readonly string _url = $"{RealeseUrl}api/FireBaseTokens";

        public async Task SendTokenToServer(string token)
        {
            var userStr = await SecureStorage.GetAsync(nameof(UserDetail));
            var _userDetail = JsonConvert.DeserializeObject<UserDetail>(userStr);
            
            if (_userDetail == null)
            {
                throw new Exception("Token value is empty");

            }
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _userDetail.Token);
                    string finalUrl = _url + $"/{token}";
                    var response = await httpClient.PostAsync(finalUrl, null);


                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"Token add to server");
                        return;
                    }
                    else
                    {
                        Debug.WriteLine($"Error: {response.StatusCode}");
                        throw new Exception("Bad responce");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General error: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Oops", "An error occurred while fetching devices.", "Ok");
                throw new Exception("Error while handling request responce");

            }
        }

        public async Task CreateToken()
        {

            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var firebaseToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

            if (!string.IsNullOrEmpty(firebaseToken))
            {
                await SendTokenToServer(firebaseToken);    
            }
            else
            {
                throw new InvalidOperationException("Firebase token could not be retrieved");
            }



        }
    }

    
}

