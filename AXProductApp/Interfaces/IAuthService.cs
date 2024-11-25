using AXProductApp.Models;

namespace AXProductApp.Interfaces;

internal interface IAuthService
{
    public Task<bool> TryLoginUser(UserLoginModel userLogin);

    public Task<bool> TryUserAutoLoggingAsync();

    public Task<bool> TryRegisterUser(UserRegisterModel userLogin);
}