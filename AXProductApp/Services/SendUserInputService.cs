using AXProductApp.Interfaces;
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
        public event Action<WindowStatus> DataReceived;
        public event Action<string> ErrorDroped;
        public SendUserInputService()
        {
            InitiaizeConnection();
        }
        public async Task<bool> InitiaizeConnection()
        {
            _hubConnection = new HubConnectionBuilder()
           .WithUrl($"{LinkToHub.RealeseUrl}/user-input-hub")
           .WithAutomaticReconnect()
           .Build();
            _hubConnection.On<WindowStatus>("ReceiveUserInputResponce", (status) =>
            {
                if (status != null)
                {
                   
                    status.TimeNow = DateTime.Now;
                    Debug.WriteLine($"User input received: {status.IsOpen} {status.IsProtected}");
                    string jsonString = JsonSerializer.Serialize(status);
                    SecureStorage.SetAsync(nameof(UserInputStatus), jsonString);
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
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending data tu hub {ex.Message}");
                ErrorDroped.Invoke(ex.Message);
                return false;
            }
        }

        public async Task OnAppUpdate()
        {
            try
            {
                string output = await SecureStorage.GetAsync(nameof(UserInputStatus));
                WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);
                if (status == null) { Debug.WriteLine("no data in user input cache"); return; }
                DataReceived.Invoke(status);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        public async Task SendOpenInfo(bool isOpened)
        {
            userInputStatus.IsOpen = isOpened;
            Debug.WriteLine($"Is open: {userInputStatus.IsOpen}");
            await  sendDataToHub(userInputStatus);
        }

        public async Task SendProtectedInfo(bool isProtected)
        {
            userInputStatus.isProtected = isProtected;
            Debug.WriteLine($"Is protected: {userInputStatus.isProtected}");
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
                ErrorDroped.Invoke(ex.Message);
            }
        }
    }
}
