﻿using System;
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
            DataReceived?.Invoke(status);
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
            TimeSpan difference = DateTime.Now - status.TimeNow ;
            if (output != null )
            {

              
                if (difference.Hours < 24){
                    status.StringTimeFromLastConnection = ($"Last connection {difference.Hours} hours ago");
                }
                
                else if (difference.Hours > 24)
                {
                    status.StringTimeFromLastConnection = ($"Last connection {difference.Days} days ago");
                }
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

}
