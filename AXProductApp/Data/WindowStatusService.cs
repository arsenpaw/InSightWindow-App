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
            .WithUrl(LinkToHub.RealeseUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<WindowStatus>("ReceiveWindowStatus", async (status) =>
        {
             DataReceived?.Invoke(status);
            string jsonString = JsonSerializer.Serialize(status);
            await SecureStorage.SetAsync(nameof(WindowStatus), jsonString);
           // await SecureStorage.SetAsync("data", DateTime.Now);


        });

        try
        {
            await _hubConnection.StartAsync();
            string output = await SecureStorage.GetAsync(nameof(WindowStatus));
            if (output == null) { throw new InvalidDataException(); }
            WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);
             DataReceived?.Invoke(status);
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                Debug.WriteLine("Connection to hub established.");             
            }
            else throw new System.Net.Http.HttpRequestException();
        }
        catch (System.Net.Http.HttpRequestException)
        {
            Console.WriteLine("No internet connectiom");
            string output = await SecureStorage.GetAsync(nameof(WindowStatus));
            
            if (output != null)
            {
                WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);
                
                Console.WriteLine(status.TimeNow.Date);
                DataReceived?.Invoke(status);
            }
            else
             NoDataAndConnection = true;
           
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error establishing connection to hub: {ex.Message}");
        }
    }

}
