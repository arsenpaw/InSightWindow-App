using AXProductApp.Interfaces;
using AXProductApp.Models.Dto;
using Google.Apis.Auth.OAuth2.Responses;

//TODO Unready page
namespace AXProductApp.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AuthApiClient _authApiClient;
    private readonly ILocalStorageService _localStorageService;

    public RefreshTokenService(AuthApiClient authApiClient, ILocalStorageService localStorageService)
    {
        _authApiClient = authApiClient;
        _localStorageService = localStorageService;
    }

    public async Task UpdateTokens() //new tokens will be writen to secure storage
    {
        var oldUser = await _localStorageService.GetUserSecret();
        if (oldUser == null)
            throw new Exception("Token value is empty");

        var responce = await _authApiClient.PostAsync<TokenResponse>("Auth/RefreshToken", new CredentialsModel
        {
            RefreshToken = oldUser.RefreshToken,
            AccessToken = oldUser.Token
        });
        oldUser.Token = responce.AccessToken;
        oldUser.RefreshToken = responce.RefreshToken;
        await _localStorageService.AddUserSecret(oldUser);
    }
}