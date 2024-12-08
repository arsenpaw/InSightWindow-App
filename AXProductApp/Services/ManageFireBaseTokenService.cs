using AXProductApp.Interfaces;
using Plugin.Firebase.CloudMessaging;
//TODO Unready page
namespace AXProductApp.Services
{
    public class ManageFireBaseTokenService : IManageFireBaseTokenService
    {
        private readonly AuthApiClient _authApiClient;
        private readonly ILocalStorageService _localStorageService;

        public ManageFireBaseTokenService(AuthApiClient authApiClient, ILocalStorageService localStorage)
        {
            _localStorageService = localStorage;
            _authApiClient = authApiClient;
        }

        private async Task SendTokenToServer(string token)
        {
            var userSecret = await _localStorageService.GetUserSecret();
            var resp = await _authApiClient.PostAsync<object>($"FireBaseTokens/{token}", null);
        }

        public async Task EnablePushNotificationForCurrentDevice()
        {
            var firebaseToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            await SendTokenToServer(firebaseToken);
        }
    }
}

