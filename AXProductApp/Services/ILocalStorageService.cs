using AXProductApp.Models;

namespace AXProductApp.Services;

public interface ILocalStorageService
{
    Task<T> GetAsync<T>(string key);
    Task<string> GetAsync(string key);
    Task SetAsync(string key, string value);
    void RemoveAsync(string key);
    void ClearAll();
    Task AddUserSecret(UserDetail userDetail);
    Task<UserDetail?> GetUserSecret();
}