using System.IdentityModel.Tokens.Jwt;
using System.Net;
using AXProductApp.Interfaces;
using AXProductApp.Models;
using AXProductApp.Models.Dto;
using Newtonsoft.Json;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AXProductApp.Services;

internal class AuthService : IAuthService
{
    private readonly AuthApiClient _httpClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(AuthApiClient httpClient, IRefreshTokenService refreshTokenService,
        ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _refreshTokenService = refreshTokenService;
        _localStorageService = localStorageService;
    }

    public async Task TryRegisterUser(UserRegisterModel userLogin)
    {
        await _httpClient.PostAsync<HttpStatusCode>("/Auth/create", userLogin);
    }

    public async Task<bool> TryUserAutoLoggingAsync()
    {
        var userData = await _localStorageService.GetUserSecret();
        if (userData == null)
            return false;
        await _refreshTokenService.UpdateTokens();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(userData.Token) as JwtSecurityToken;

        //TODO create separete storfage and jwt service to handle all that stuff
        if (jwtToken.ValidTo > DateTime.UtcNow)
            return true;

        await Application.Current.MainPage.DisplayAlert("Oops", "Something went wrong during auto-login",
            "Try Again");
        return false;
    }

    public async Task<bool> TryLoginUser(UserLoginModel userLogin)
    {
        var response = await _httpClient.PostAsync<CredentialsModel>("/Auth/login", userLogin);
        await WriteTokenDataToStorage(response.AccessToken, response.RefreshToken);
        return true;
    }

    public async Task WriteTokenDataToStorage(string jwtToken, string refreshToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;
        var userId = jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId)?.Value;

        var user = new UserDetail
        {
            Token = jwtToken,
            Id = userId,
            RefreshToken = refreshToken
        };

        var userStr = JsonConvert.SerializeObject(user);
        await SecureStorage.SetAsync(nameof(UserDetail), userStr);
    }
}