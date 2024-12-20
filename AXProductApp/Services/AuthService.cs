﻿using AXProductApp.Interfaces;
using AXProductApp.Models;
using AXProductApp.Models.Dto;
using Google.Apis.Auth.OAuth2.Responses;
using System.IdentityModel.Tokens.Jwt;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AXProductApp.Services;

internal class AuthService : IAuthService
{
    private readonly AuthApiClient _httpClient;
    private readonly ILocalStorageService _localStorageService;
    private readonly IManageFireBaseTokenService _manageFireBaseTokenService;
    public AuthService(AuthApiClient httpClient,
        ILocalStorageService localStorageService,
        IManageFireBaseTokenService manageFireBaseTokenService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
        _manageFireBaseTokenService = manageFireBaseTokenService;
    }

    public async Task<bool> TryRegisterUser(UserRegisterModel userLogin)
    {
        var responce = await _httpClient.PostAsync<object>("Auth/create", userLogin);
        return responce.IsSuccess;
    }

    public async Task<bool> TryUserAutoLoggingAsync()
    {
        var user = await _localStorageService.GetUserSecret();
        if (user == null)
        {
            return false;
        }

        var responce = await _httpClient.PostAsync<TokenResponse>("Auth/RefreshToken", new TokenResponse
        {
            AccessToken = user.Token,
            RefreshToken = user.RefreshToken
        });
        if (responce.IsSuccess)
        {
            await WriteTokenDataToStorage(responce.Data.AccessToken, responce.Data.RefreshToken);
        }

        return responce.IsSuccess;
    }

    public async Task<bool> TryLoginUser(UserLoginModel userLogin)
    {
        var response = await _httpClient.PostAsync<CredentialsModel>("Auth/Login", userLogin);
        if (!response.IsSuccess)
        {
            return false;
        }

        var credentials = response.Data;
        await WriteTokenDataToStorage(credentials.AccessToken, credentials.RefreshToken);
        await _manageFireBaseTokenService.EnablePushNotificationForCurrentDevice();
        return true;
    }

    private async Task WriteTokenDataToStorage(string jwtToken, string refreshToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;
        var userId = jsonToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
        var role = jsonToken.Claims.FirstOrDefault(x => x.Type == "role")?.Value;
        var user = new UserDetail
        {
            Token = jwtToken,
            Id = userId,
            Role = role,
            RefreshToken = refreshToken
        };

        await _localStorageService.AddUserSecret(user);
    }
}