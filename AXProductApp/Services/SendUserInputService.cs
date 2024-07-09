using AXProductApp.Interfaces;
using AXProductApp.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AXProductApp.Data
{
    public class SendUserInputService : ISendUserInputService
    {
        UserInputStatus userInputStatus = new UserInputStatus();

        public HubConnection _hubConnection;

        private Guid _deviceId; 

        public event Action<UserInputStatus> DataReceived;

        public async Task<bool> InitiaizeConnectionAsync(Guid deviceId)
        {
            _hubConnection = new HubConnectionBuilder()
           .WithUrl($"{LinkToHub.RealeseUrl}user-input-hub")
           .WithAutomaticReconnect()
           .Build();
            _hubConnection.On<UserInputStatus>("ReceiveUserInputResponce", (status) =>
            {
                if (status != null)
                {

                    Debug.WriteLine($"User input received: {status.IsOpenButton} {status.IsProtectedButton}");
                    DataReceived.Invoke(status);

                }
                else
                {
                    Debug.WriteLine("No data received");
                }

            });
            try
            {
                await _hubConnection.StartAsync();
                _deviceId = deviceId;
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending data tu hub {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Oops", "An error occurred while sendig youre command ", "Ok");
                return false;
            }
        }

        //public async Task OnAppUpdate()
        //{
        //    try
        //    {
        //        string output = await SecureStorage.GetAsync(nameof(UserInputStatus));
        //        WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);
        //        if (status == null) { Debug.WriteLine("no data in user input cache"); return; }
        //        DataReceived.Invoke(status);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //    }
        //}
        public async Task SendOpenInfo(bool isOpened)
        {
            userInputStatus.IsOpenButton = isOpened;
            userInputStatus.DeviceId = _deviceId;    
            Debug.WriteLine($"Is open: {userInputStatus.IsOpenButton}");
            await sendDataToHub(userInputStatus);
        }

        public async Task SendProtectedInfo(bool isProtected)
        {
            userInputStatus.IsProtectedButton = isProtected;
            userInputStatus.DeviceId = _deviceId;
            Debug.WriteLine($"Is protected: {userInputStatus.IsProtectedButton}");
            await sendDataToHub(userInputStatus);
        }
        public async Task sendDataToHub(UserInputStatus userInputStatus)
        {
            try
            {
                if (_hubConnection.State == HubConnectionState.Connected)
                    await _hubConnection.SendAsync("SaveUserInput", userInputStatus);
                else
                    throw new Exception("FAILED CONECT TO HUB");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in sendind data to hub {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Oops", "An error occurred while sendig youre command ", "Ok");
            }
        }
    }
}
