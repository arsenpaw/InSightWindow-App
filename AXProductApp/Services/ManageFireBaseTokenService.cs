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
        public async Task SendTokenToServer(string token)
        {
            var userSecret = await _localStorageService.GetUserSecret();
            var resp = await _authApiClient.PostAsync<object>($"FireBaseTokens/{token}", null);
        }

        public async Task CreateToken()
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            try
            {
                var firebaseToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                if (!string.IsNullOrEmpty(firebaseToken))
                {
                    await SendTokenToServer(firebaseToken);
                }
                else
                {
                    throw new InvalidOperationException("Firebase token could not be retrieved");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }


}

