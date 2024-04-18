﻿
namespace AXProductApp.Data
{
    public interface ISendUserInputService
    {
        event Action<WindowStatus> DataReceived;

        Task<bool> InitiaizeConnection();
        Task SendOpenInfo(bool isOpened);
        Task SendProtectedInfo(bool isProtected);
    }
}