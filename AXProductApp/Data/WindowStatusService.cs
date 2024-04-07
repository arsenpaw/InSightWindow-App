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
            string jsonString = JsonSerializer.Serialize(status);
            await SecureStorage.SetAsync(nameof(WindowStatus), jsonString);
          
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
            Debug.WriteLine("No internet connectiom");
            // тут юра
        }
        catch (InvalidDataException ex)
        {
            Debug.WriteLine($"No data retrived from cahce {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error ocuured: {ex.Message}");
        }
    }

}
