using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using AXProductApp.Data;
using Blazored.LocalStorage;
using System.IO;
using System.Text.Json;
using Plugin.LocalNotification;
using AXProductApp.Services;
using Microsoft.Maui.Controls;
namespace AXProductApp.Data
{

    public class ReceiveWindowStatusService : IReceiveWindowStatusService
    {
        private bool prevAlarmTriggered;    

        private HubConnection _hubConnection;

        public event Action<WindowStatus> DataReceived;

        public bool NoDataAndConnection = false;

        public ReceiveWindowStatusService()
        {
            InitializeConnection();
            
        }
       
        public async Task<bool> InitializeConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(LinkToHub.RealeseUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<WindowStatus>("ReceiveWindowStatus", async (status) =>
            {
                if (status.isAlarm.ToBool() && prevAlarmTriggered != true)
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



