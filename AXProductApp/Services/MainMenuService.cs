using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AXProductApp.Interfaces;
using AXProductApp.Models;
using AXProductApp.Models.Dto;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using static AXProductApp.Data.LinkToHub;

namespace AXProductApp.Services
{
    public class MainMenuService : IMainMenu
    {

        private readonly string _url = $"{RealeseUrl}api/DevicesDb/DeviceOfUser";

        public async Task OnAppUpdateAsync()
        {
            await GetUserDevicesAsync();
        }

  


        public async Task<List<DeviceDto>> GetUserDevicesAsync()
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
                    var response = await httpClient.GetAsync(_url);

                    if (response.IsSuccessStatusCode)
                    {
                        var responceBody = await response.Content.ReadAsStringAsync();
                        var devicesList = JsonConvert.DeserializeObject<List<DeviceDto>>(responceBody);
                        return devicesList;
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
    }
}
