using AXProductApp.Models;

namespace AXProductApp.Interfaces
{
    public interface ISendUserInputService
    {
        event Action<UserInputStatus> DataReceived;

        Task<bool> InitiaizeConnectionAsync(Guid deviceId);

        Task SendOpenInfo(bool isOpened);

        Task SendProtectedInfo(bool isProtected);

        Task sendDataToHub(UserInputStatus userInputStatus);

    }
}