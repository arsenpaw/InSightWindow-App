using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AXProductApp.Services;

public class AuthApiClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions;

    public AuthApiClient(string baseAddress)
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri(baseAddress.Trim());

       // _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public void SetAuthorizationHeader(string token)
    {
        if (!string.IsNullOrEmpty(token))
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _client.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"Error: {response.StatusCode}, Message: {await response.Content.ReadAsStringAsync()}");

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, _serializerOptions);
    }

    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        var jsonData = JsonSerializer.Serialize(data, _serializerOptions);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        Console.WriteLine(_client.BaseAddress);
        var response = await _client.PostAsync(endpoint, content);

  
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"Error: {response.StatusCode}, Message: {await response.Content.ReadAsStringAsync()}");

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, _serializerOptions);
    }

    public async Task<T> PutAsync<T>(string endpoint, object data)
    {
        var jsonData = JsonSerializer.Serialize(data, _serializerOptions);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var t = _client.BaseAddress + endpoint;
        var response = await _client.PutAsync(endpoint, content);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"Error: {response.StatusCode}, Message: {await response.Content.ReadAsStringAsync()}");

        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContent, _serializerOptions);
    }


    public async Task<bool> DeleteAsync(string endpoint)
    {
        var response = await _client.DeleteAsync(endpoint);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"Error: {response.StatusCode}, Message: {await response.Content.ReadAsStringAsync()}");

        return true;
    }
}