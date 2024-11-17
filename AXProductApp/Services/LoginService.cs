using System.IdentityModel.Tokens.Jwt;
using System.Net;
using AXProductApp.Interfaces;
using AXProductApp.Models;
using AXProductApp.Models.Dto;
using Newtonsoft.Json;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AXProductApp.Services;

internal class LoginService : ILoginService
{
    private readonly AuthApiClient _httpClient;
    private readonly IRefreshTokenService _refreshTokenService;

    public LoginService(AuthApiClient httpClient, IRefreshTokenService refreshTokenService)
    {
        _httpClient = httpClient;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<bool> TryUserAutoLoggingAsync()
    {
        var oldUserStr = await SecureStorage.GetAsync(nameof(UserDetail));

        if (!string.IsNullOrWhiteSpace(oldUserStr))
        {
            var statusCode = await _refreshTokenService.UpdateTokens();
            var user = JsonConvert.DeserializeObject<UserDetail>(oldUserStr);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(user.Token) as JwtSecurityToken;

            //TODO create separete storfage and jwt service to handle all that stuff
            if (jwtToken.ValidTo > DateTime.UtcNow && statusCode == HttpStatusCode.OK)
                return true;
            await Application.Current.MainPage.DisplayAlert("Oops", "Something went wrong during auto-login",
                "Try Again");
        }

        return false;
    }

    public async Task<bool> TryAuthenticateUserAsync(UserLoginModel userLogin)
    {
        var response = await _httpClient.PostAsync<TokenResponse>("Auth/login", userLogin);
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