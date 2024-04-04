using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using AXProductApp.Data;
using Blazored.LocalStorage;
using System.IO;

public class SignalRService
{
 
    private HubConnection _hubConnection;

    public event Action<WindowStatus> DataReceived;


    public async Task InitializeConnection()
    {
        _hubConnection = new HubConnectionBuilder()
            //.WithUrl(new Uri("http://192.168.4.2:81/client-hub")) // This URL should match your SignalR hub endpoint
            .WithUrl(new Uri("https://localhost:44324/client-hub")) 
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<WindowStatus>("ReceiveWindowStatus", async (status) =>
        {
            DataReceived?.Invoke(status);
        });

        try
        {
            await _hubConnection.StartAsync();
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                Debug.WriteLine("Connection to hub established.");             
            }
            else throw new System.Net.Http.HttpRequestException();
        }
        catch (System.Net.Http.HttpRequestException)
        {
            Console.WriteLine("No internet connectiom");
            // тут юра
           
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error establishing connection to hub: {ex.Message}");
        }
    }

}
