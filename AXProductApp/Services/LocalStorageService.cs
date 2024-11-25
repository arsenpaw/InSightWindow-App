using AXProductApp.Models;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AXProductApp.Services;

public class LocalStorageService : ILocalStorageService
{
    public async Task<string> GetAsync(string key)
    {
        return await SecureStorage.GetAsync(key);
    }

    public async Task SetAsync(string key, string value)
    {
        await SecureStorage.SetAsync(key, value);
    }

    public void RemoveAsync(string key)
    {
        SecureStorage.Remove(key);
    }

    public void ClearAll()
    {
        SecureStorage.RemoveAll();
    }

    public async Task AddUserSecret(UserDetail userDetail)
    {
        SecureStorage.Remove(nameof(UserDetail));
        await SecureStorage.SetAsync(nameof(UserDetail), JsonSerializer.Serialize(userDetail));
    }

    public async Task<UserDetail?> GetUserSecret()
    {
        
        var strData = await SecureStorage.GetAsync(nameof(UserDetail));
        if (string.IsNullOrEmpty(strData))
            return null;
        return JsonConvert.DeserializeObject<UserDetail>(strData);
    }
}