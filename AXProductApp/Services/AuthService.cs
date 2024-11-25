using System.IdentityModel.Tokens.Jwt;
using AXProductApp.Interfaces;
using AXProductApp.Models;
using AXProductApp.Models.Dto;
using Google.Apis.Auth.OAuth2.Responses;
using Java.Lang;
using Newtonsoft.Json;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AXProductApp.Services;

internal class AuthService : IAuthService
{
    private readonly AuthApiClient _httpClient;
    private readonly ILocalStorageService _localStorageService;

    public AuthService(AuthApiClient httpClient,
        ILocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
    }

    public async Task<bool> TryRegisterUser(UserRegisterModel userLogin)
    {
        var responce = await _httpClient.PostAsync<StringBuilder>("/Auth/create", userLogin);
        return responce.IsSuccess;
    }

    public async Task<bool> TryUserAutoLoggingAsync() //new tokens will be writen to secure storage
    {
        var user = await _localStorageService.GetUserSecret();
        if (user == null)
            return false;
        var responce = await _httpClient.PostAsync<TokenResponse>("Auth/RefreshToken", new TokenResponse
        {
            AccessToken = user.Token,
            RefreshToken = user.RefreshToken
        });
        if (responce.IsSuccess)
            await WriteTokenDataToStorage(responce.Data.AccessToken, responce.Data.RefreshToken);

        return responce.IsSuccess;
    }

    public async Task<bool> TryLoginUser(UserLoginModel userLogin)
    {
        var response = await _httpClient.PostAsync<CredentialsModel>("Auth/Login", userLogin);
        if (!response.IsSuccess)
            return false;
        var credentials = response.Data;
        await WriteTokenDataToStorage(credentials.AccessToken, credentials.RefreshToken);
        return true;
    }

    private async Task WriteTokenDataToStorage(string jwtToken, string refreshToken)
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