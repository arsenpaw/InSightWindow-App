using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using AXProductApp.Interfaces;
using AXProductApp.Models.Dto;
using Newtonsoft.Json;
using static AXProductApp.Data.LinkToHub;

namespace AXProductApp.Services
{
    public class MainMenuService : IMainMenu
    {

        private readonly string _baseUrl = $"{RealeseUrl}api/DevicesDb/DeviceOfUser";
        private string _url;
        private string _token = string.Empty;

        public MainMenuService()
        {
            GrabUserParametersAsync();
        }

        private async void GrabUserParametersAsync()
        {
            var userId = await SecureStorage.GetAsync("userId");
            _url = $"{_baseUrl}/{userId}";
       
        }

        public async Task OnAppUpdateAsync()
        {
            await GetUserDevicesAsync();
        }

        public async Task<List<DeviceDto>> GetUserDevicesAsync()
        {
            _token = await SecureStorage.GetAsync("token");
            Debug.Write(_token);

            if (string.IsNullOrEmpty(_token))
            {
                throw new Exception("Token value is empty");
            }

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                    var response = await httpClient.GetAsync(_url);

                    if (response.IsSuccessStatusCode)
                    {
                        var responceBody = await response.Content.ReadAsStringAsync();
                        var devicesList= JsonConvert.DeserializeObject<List<DeviceDto>>(responceBody);
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
                throw new Exception("Bad responce");
                
            }
        }
    }
    }
