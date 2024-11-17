using AXProductApp.Models;

namespace AXProductApp.Interfaces;

internal interface IRegisterService
{
    public Task<string> AuthenticateUser(UserRegisterModel userLogin);
}