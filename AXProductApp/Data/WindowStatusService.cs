using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  AXProductApp.Data;
using Microsoft.AspNetCore.SignalR.Client;


namespace AXProductApp.Data
{
    public class WindowStatusService
    {
        public WindowStatusService()
        {
            var hubConnection = new HubConnectionBuilder()
           .WithUrl("wss://localhost:7009/client-hub")
           .Build();

            // Define the method to be called from the server
            hubConnection.On<string>("SendWindowStatus", (message) =>
            {
                Console.WriteLine($"Received message: {message}");
            });
        }
        public Task<WindowStatus> ReceiveWindowStatus(WindowStatus windowStatus)
        {
            Console.WriteLine($"{windowStatus.Temparature}");
           //write hello 

            //show all 

            return Task.FromResult(windowStatus);
        }
    }
}
