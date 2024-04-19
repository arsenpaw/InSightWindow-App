
namespace AXProductApp.Data
{
    public interface IReceiveWindowStatusService
    {
        event Action<WindowStatus> DataReceived;

        Task<bool> InitializeConnection();
        Task OnAppUpdate();
    }
}