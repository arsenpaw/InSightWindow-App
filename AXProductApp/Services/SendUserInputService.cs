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
    public class SendUserInputService
    {
        UserInputStatus userInputStatus = new UserInputStatus();
        private HubConnection _hubConnection;
        public SendUserInputService()
        {
           InitiaizeConnection();
        }
        private async Task<bool> InitiaizeConnection()
        {
            _hubConnection = new HubConnectionBuilder()
           .WithUrl(LinkToHub.RealeseUrlInput)
           .WithAutomaticReconnect()
           .Build();
            try
            {
                await _hubConnection.StartAsync();   
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending data tu hub {ex.Message}");
                return false;
            }
        }
  
        public async Task SendOpenInfo(bool isOpened)
        {
            userInputStatus.IsOpen = isOpened;
            Debug.WriteLine($"Is open: {userInputStatus.IsOpen}");
            await sendDataToHub(userInputStatus);
        }

        public async Task SendProtectedInfo(bool isProtected)
        {
            userInputStatus.isProtected = isProtected;
            Debug.WriteLine($"Is protected: {userInputStatus.isProtected}");
            await sendDataToHub(userInputStatus);
        }
        private async Task sendDataToHub(UserInputStatus userInputStatus)
        {
            try
            {
                if (_hubConnection.State == HubConnectionState.Connected)
                    await _hubConnection.SendAsync("SaveUserInput", userInputStatus);
                else if (_hubConnection.State == HubConnectionState.Connecting)
                    throw new Exception("FAILED TO CONECT TO HUB");
                else
                {
                    if (await InitiaizeConnection())
                        await sendDataToHub(userInputStatus);
                    else
                        throw new Exception("FAILED TO CONECT TO HUB");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in sendind data to hub {ex.Message}");
            }
        }
    }
}
