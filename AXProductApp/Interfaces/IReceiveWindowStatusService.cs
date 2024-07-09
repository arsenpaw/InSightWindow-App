using AXProductApp.Models;

namespace AXProductApp.Interfaces
{
    public interface IReceiveWindowStatusService
    {
        event Action<WindowStatus> DataReceived;

        Task<bool> InitializeConnectionAsync(Guid deviceId);

        Task OnAppUpdate();
    }
}