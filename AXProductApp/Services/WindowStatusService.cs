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
            .WithUrl(LinkToHub.ArsenTest)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<WindowStatus>("ReceiveWindowStatus", async (status) =>
        {
           
            status.TimeNow = DateTime.Now;
            
            string jsonString = JsonSerializer.Serialize(status);
            await SecureStorage.SetAsync(nameof(WindowStatus), jsonString);
            status.StringTimeFromLastConnection = await LastConnectInfo(status);
            DataReceived?.Invoke(status);

        });

        try
        {
            await _hubConnection.StartAsync();
            string output = await SecureStorage.GetAsync(nameof(WindowStatus));
            if (output == null) { throw new InvalidDataException(); }
            WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);
            TimeSpan difference = DateTime.Now - status.TimeNow;
            status.StringTimeFromLastConnection = await LastConnectInfo(status);
           
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                DataReceived?.Invoke(status);
                Debug.WriteLine("Connection to hub established.");
            }
            else throw new HttpRequestException();
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("No internet connectiom");
            string output = await SecureStorage.GetAsync(nameof(WindowStatus));
            WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);
           
            status.StringTimeFromLastConnection = await LastConnectInfo(status);
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

    public async Task<string > LastConnectInfo(WindowStatus status)
    {
        string ReturnString;
        TimeSpan TimeDiff = DateTime.Now - status.TimeNow;
        if (TimeDiff.TotalMinutes <= 1)
        {
            ReturnString = "Online";
        }
        else if (TimeDiff.TotalMinutes <= 59 && TimeDiff.TotalMinutes > 1)
        {
            ReturnString = ($"Last connection {TimeDiff.Minutes} minutes ago");
        }
        else if (TimeDiff.TotalHours < 24)
        {
            ReturnString = ($"Last connection {TimeDiff.Hours} hours ago");
        }
        else
        {
            ReturnString = ($"Last connection {TimeDiff.Days} days ago");
        }

        return ReturnString;


    }

}
