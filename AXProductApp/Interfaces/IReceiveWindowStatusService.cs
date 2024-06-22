using AXProductApp.Data;

namespace AXProductApp.Interfaces
{
    public interface IReceiveWindowStatusService
    {
        event Action<WindowStatus> DataReceived;

        Task<bool> InitializeConnection();
        Task OnAppUpdate();
    }
}