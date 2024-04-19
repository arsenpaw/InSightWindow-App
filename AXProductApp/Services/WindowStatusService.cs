using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using AXProductApp.Data;
using Blazored.LocalStorage;
using System.IO;
using System.Text.Json;
using Plugin.LocalNotification;


namespace AXProductApp.Data
{

    public class ReceiveWindowStatusService : IReceiveWindowStatusService
    {

        private HubConnection _hubConnection;

        public event Action<WindowStatus> DataReceived;

        public bool NoDataAndConnection = false;

        public ReceiveWindowStatusService()
        {
            InitializeConnection();
        }
        private void TestSms()
        {
            var request = new NotificationRequest
            {
                NotificationId = 1337,
                Title = "MEDIUM",
                Subtitle = "Hello! I'm Erdal",
                Description = "Local Push Notification",
                BadgeNumber = 1,

                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(3),
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }
        public async Task<bool> InitializeConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(LinkToHub.WIFI)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<WindowStatus>("ReceiveWindowStatus", async (status) =>
            {
                status.TimeNow = DateTime.Now;
                string jsonString = JsonSerializer.Serialize(status);
                await SecureStorage.SetAsync(nameof(WindowStatus), jsonString);
                DataReceived?.Invoke(status);
                TestSms();
            });

            try
            {
                await _hubConnection.StartAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error establishing connection to hub: {ex.Message}\n {ex.InnerException} \n{ex.Data}");
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
                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    DataReceived?.Invoke(status);
                    Debug.WriteLine("Connection to hub established.");
                }
                else throw new HttpRequestException();
            }
            catch (HttpRequestException)
            {
                Debug.WriteLine("No internet connectiom");
                string output = await SecureStorage.GetAsync(nameof(WindowStatus));
                WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);

                if (output != null)
                {
                    DataReceived?.Invoke(status);
                }
                else
                    NoDataAndConnection = true;
                Debug.WriteLine("No data in cashe or no connection");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting data from cache: {ex.Message}");
            }
        }


    }
}



