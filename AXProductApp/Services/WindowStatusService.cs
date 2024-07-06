using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Blazored.LocalStorage;
using System.IO;
using System.Text.Json;
using Plugin.LocalNotification;
using AXProductApp.Services;
using Microsoft.Maui.Controls;
using AXProductApp.Interfaces;
namespace AXProductApp.Data
{

    public class ReceiveWindowStatusService : IReceiveWindowStatusService
    {
        private bool prevAlarmTriggered;

        public HubConnection _hubConnection;

        public event Action<WindowStatus> DataReceived;




        public ReceiveWindowStatusService()
        {
            InitializeConnection();

        }

        public async Task<bool> InitializeConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                  .WithUrl($"{LinkToHub.RealeseUrl}/user-hub")
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<WindowStatus>("ReceiveWindowStatus", async (status) =>
            {
                if (status.isAlarm.ToBool() && prevAlarmTriggered == false)
                {
                    prevAlarmTriggered = true;
                    new NotificationService().sendAlarmMessage();
                }
                else if (!status.isAlarm.ToBool())
                {
                    prevAlarmTriggered = false;
                }
                status.TimeNow = DateTime.Now;
                string jsonString = JsonSerializer.Serialize(status);
                await SecureStorage.SetAsync(nameof(WindowStatus), jsonString);
                DataReceived?.Invoke(status);

            });

            try
            {
                await _hubConnection.StartAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error establishing connection to hub: {ex.Message}\n {ex.InnerException} \n{ex.Data}");
                await App.Current.MainPage.DisplayAlert("Oops", "An error occurred while receiving widnow info", "Ok");
                return false;
            }

        }
        public async Task OnAppUpdate()
        {
            try
            {
                string output = await SecureStorage.GetAsync(nameof(WindowStatus));
                if (output == null) { throw new InvalidDataException(); }
                WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);
                DataReceived?.Invoke(status);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting data from cache: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Oops", "An error occurred while receiving widnow info", "Ok");
            }
        }


    }
}



