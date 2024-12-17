using System.Diagnostics;
using AXProductApp.Interfaces;
using AXProductApp.Models;
using AXProductApp.Models.Command;
using AXProductApp.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;


//TODO Unready page
namespace AXProductApp.Data;

public class ReceiveWindowStatusService : IReceiveWindowStatusService
{
    public HubConnection _hubConnection;

    private bool prevAlarmTriggered;

    public event Action<AllWindowDataDto> DataReceived;
    public event Action<UserInputStatus> UserReceived;

    public async Task<bool> InitializeConnectionAsync(Guid deviceId)
    {
        var userDetail = JsonConvert.DeserializeObject<UserDetail>(await SecureStorage.GetAsync(nameof(UserDetail)));
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://axproduct-server.azurewebsites.net/client-hub",
                options => { options.AccessTokenProvider = () => Task.FromResult(userDetail.Token); })
            .WithAutomaticReconnect()
            .Build();

        await _hubConnection.StartAsync();

        return true;
    }

    public async Task ReceiveSensorData(Guid deviceId)
    {
        _hubConnection.On<AllWindowDataDto>("ReceiveSensorData", async status =>
        {

            if (status.IsAlarm && prevAlarmTriggered == false)
            {
                prevAlarmTriggered = true;
                new NotificationService().sendAlarmMessage();
            }
            else if (!status.IsAlarm)
            {
                prevAlarmTriggered = false;
            }

            //status.TimeNow = DateTime.Now;
            //var jsonString = JsonConvert.SerializeObject(status);
            //await SecureStorage.SetAsync(status.Id.ToString(), jsonString);

            DataReceived?.Invoke(status);
        });
    }


    public async Task SendCommand(Guid deviceId, CommandDto command)
    {
        if (_hubConnection.State == HubConnectionState.Connected)
        {
            await App.Current.MainPage.DisplayAlert("Ok", "Ok", "Ok");
            await _hubConnection.SendAsync("SendCommandToEsp32", deviceId, command);
        }
        else
        {
            Debug.WriteLine("Error: No cotect to hub");
            await App.Current.MainPage.DisplayAlert("Oops", "An error occurred while sendig youre command ", "Ok");
        }
    }


    public async Task StopConnection(Guid deviceId)
    {

    }











    public async Task ReceiveSensorDataTest(Guid deviceId)
    {
        // Перенесені значить на сторінку
        AllWindowDataDto statusTest = new AllWindowDataDto();
        statusTest.Temperature = 10;
        statusTest.Humidity = 10;
        statusTest.IsRain = false;
        statusTest.IsOpen = true;
        statusTest.IsAlarm = true;
        DataReceived?.Invoke(statusTest);
    }





    //public async Task TryShowDataFromCashe(string deviceId)
    //{
    //    var strData = await SecureStorage.GetAsync(deviceId);
    //    if (strData != null)
    //    {
    //        var objDataFromCache = JsonConvert.DeserializeObject<AllWindowDataDto>(strData);
    //        DataReceived?.Invoke(objDataFromCache);
    //    }
    //}


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