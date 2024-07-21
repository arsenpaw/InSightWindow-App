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
using AXProductApp.Models;
using Newtonsoft.Json;
namespace AXProductApp.Data
{

    public class ReceiveWindowStatusService : IReceiveWindowStatusService
    {
        private bool prevAlarmTriggered;

        public HubConnection _hubConnection;

        public event Action<AllWindowDataDto> DataReceived;
       

        public async Task<bool> InitializeConnectionAsync(Guid deviceId)
        {
            var userDetail = JsonConvert.DeserializeObject<UserDetail>(await SecureStorage.GetAsync(nameof(UserDetail)));
            _hubConnection = new HubConnectionBuilder()

            .WithUrl($"{LinkToHub.RealeseUrl}client-hub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(userDetail.Token);
            })                 
            .WithAutomaticReconnect()
            .Build();

            try
            {
                await _hubConnection.StartAsync();
                    _hubConnection.On<AllWindowDataDto>("ReceiveWindowStatus", async (status) =>
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
                        string jsonString = JsonConvert.SerializeObject(status);
                        await SecureStorage.SetAsync(status.Id.ToString(), jsonString);
                        
                        DataReceived?.Invoke(status);

                    });
               return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error establishing connection to hub: {ex.Message}\n {ex.InnerException} \n{ex.Data}");
                await App.Current.MainPage.DisplayAlert("Oops", "An error occurred while establishing connection to hub", "Ok");
                return false;
            }
        }

        public async Task TryShowDataFromCashe(string deviceId)
        {
            var strData = await SecureStorage.GetAsync(deviceId);
            if (strData != null)
            {
               var objDataFromCache =  JsonConvert.DeserializeObject<AllWindowDataDto>(strData);
                DataReceived?.Invoke(objDataFromCache);
            }
        }

        //public async Task OnAppUpdate()
        //{
        //    try
        //    {
        //        string output = await SecureStorage.GetAsync(nameof(WindowStatus));
        //        if (output == null) { throw new InvalidDataException(); }
        //        WindowStatus status = JsonConvert.DeserializeObject<WindowStatus>(output);
        //        DataReceived?.Invoke(status);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Error getting data from cache: {ex.Message}");
        //        await App.Current.MainPage.DisplayAlert("Oops", "An error occurred while receiving widnow info", "Ok");
        //    }
        //}


    }
}



