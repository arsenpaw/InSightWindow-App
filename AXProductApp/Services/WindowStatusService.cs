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
            DataReceived?.Invoke(status);

        });

        try
        {
            await _hubConnection.StartAsync();
            string output = await SecureStorage.GetAsync(nameof(WindowStatus));
            if (output == null) { throw new InvalidDataException(); }
            WindowStatus status = JsonSerializer.Deserialize<WindowStatus>(output);
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
            Debug.WriteLine($"Error establishing connection to hub: {ex.Message}\n {ex.InnerException} \n{ex.Data}");
        }

    }



}
