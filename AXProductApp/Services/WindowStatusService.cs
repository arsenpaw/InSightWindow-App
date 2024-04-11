using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using AXProductApp.Data;
using Blazored.LocalStorage;
using System.IO;
using System.Text.Json;


public class SignalRService
{
 
    private HubConnection _hubConnection;

    public event Action<WindowStatus> DataReceived;

    public bool NoDataAndConnection = false;


   
    public async Task InitializeConnection()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(LinkToHub.RomaTest)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<WindowStatus>("ReceiveWindowStatus", async (status) =>
        {
           
            status.TimeNow = DateTime.Now;
            DataReceived?.Invoke(status);
            string jsonString = JsonSerializer.Serialize(status);
           
            await SecureStorage.SetAsync(nameof(WindowStatus), jsonString);
           
            
        });

        try
        {
            await _hubConnection.StartAsync();
            string output = await SecureStorage.GetAsync(nameof(WindowStatus));
            if (output == null) { throw new InvalidDataException(); }
            WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);
            TimeSpan difference = DateTime.Now - status.TimeNow;
            DataReceived?.Invoke(status);
            status.StringTimeFromLastConnection = LastConnectInfo(difference);
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                Debug.WriteLine("Connection to hub established.");
            }
            else throw new HttpRequestException();
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("No internet connectiom");
            string output = await SecureStorage.GetAsync(nameof(WindowStatus));
            WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);
            if (output != null )
            {

                DataReceived?.Invoke(status);
               
            }
            else
             NoDataAndConnection = true;
             Console.WriteLine("No data in cashe or no connection");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error establishing connection to hub: {ex.Message}\n {ex.InnerException} \n{ex.Data}");
        }

    }

    public string LastConnectInfo(TimeSpan TimeDiff)
    {
        string ReturnString;

        if (TimeDiff.Minutes < 5)
        {
            ReturnString = "Online";
        }
        else if (TimeDiff.Minutes <= 59 || TimeDiff.Minutes > 5)
        {
            ReturnString = ($"Last connection {TimeDiff.Minutes} Minutes ago");
        }
        else if (TimeDiff.Hours < 24)
        {
            ReturnString = ($"Last connection {TimeDiff.Hours} Hours ago");
        }
        else
        {
            ReturnString = ($"Last connection {TimeDiff.Days} Days ago");
        }

        return ReturnString;


    }

}
