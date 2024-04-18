
namespace AXProductApp.Data
{
    public interface IReceiveWindowStatusService
    {
        event Action<WindowStatus> DataReceived;

        Task InitializeConnection();
        Task OnAppUpdate();
    }
}