using System.Diagnostics;
using AXProductApp.Interfaces;
using AXProductApp.Models;
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


    public async Task<bool> InitializeConnectionAsync(Guid deviceId)
    {
        var userDetail = JsonConvert.DeserializeObject<UserDetail>(await SecureStorage.GetAsync(nameof(UserDetail)));
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{"LinkToHub"}client-hub",
                options => { options.AccessTokenProvider = () => Task.FromResult(userDetail.Token); })
            .WithAutomaticReconnect()
            .Build();

        try
        {
            await _hubConnection.StartAsync();
            _hubConnection.On<AllWindowDataDto>("ReceiveWindowStatus", async status =>
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
                var jsonString = JsonConvert.SerializeObject(status);
                await SecureStorage.SetAsync(status.Id.ToString(), jsonString);

                DataReceived?.Invoke(status);
            });
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error establishing connection to hub: {ex.Message}\n {ex.InnerException} \n{ex.Data}");
            await Application.Current.MainPage.DisplayAlert("Oops",
                "An error occurred while establishing connection to hub", "Ok");
            return false;
        }
    }

    public async Task TryShowDataFromCashe(string deviceId)
    {
        var strData = await SecureStorage.GetAsync(deviceId);
        if (strData != null)
        {
            var objDataFromCache = JsonConvert.DeserializeObject<AllWindowDataDto>(strData);
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