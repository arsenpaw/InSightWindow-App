using AXProductApp.Data;

namespace AXProductApp.Interfaces
{
    public interface ISendUserInputService
    {
        event Action<WindowStatus> DataReceived;

        Task<bool> InitiaizeConnection();

        Task SendOpenInfo(bool isOpened);

        Task SendProtectedInfo(bool isProtected);

        Task sendDataToHub(UserInputStatus userInputStatus);

    }
}