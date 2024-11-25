using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AXProductApp.Services;

public class HttpResponseModel<T>
{
    public HttpResponseModel(T data, HttpStatusCode statusCode, bool isSuccess)
    {
        Data = data;
        StatusCode = statusCode;
        IsSuccess = isSuccess;
    }

    public T Data { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; }
}

public class AuthApiClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _serializerOptions;

    public AuthApiClient(string baseAddress)
    {
        _client = new HttpClient { BaseAddress = new Uri(baseAddress.Trim()) };
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

    public async Task<HttpResponseModel<T>> GetAsync<T>(string endpoint) where T : new()
    {
        return await SendRequestAsync<T>(HttpMethod.Get, endpoint);
    }

    public async Task<HttpResponseModel<T>> PostAsync<T>(string endpoint, object data) where T : new()
    {
        return await SendRequestAsync<T>(HttpMethod.Post, endpoint, data);
    }

    public async Task<HttpResponseModel<T>> PutAsync<T>(string endpoint, object data) where T : new()
    {
        return await SendRequestAsync<T>(HttpMethod.Put, endpoint, data);
    }

    public async Task<HttpStatusCode> DeleteAsync(string endpoint)
    {
        var response = await _client.DeleteAsync(endpoint);
        return response.StatusCode;
    }

    private async Task<HttpResponseModel<T>> SendRequestAsync<T>(HttpMethod method, string endpoint,
        object? data = null) where T : new()
    {
        var request = new HttpRequestMessage(method, endpoint);

        if (data != null)
        {
            var jsonData = JsonSerializer.Serialize(data, _serializerOptions);
            request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        }

        var response = await _client.SendAsync(request);

        if (!response.IsSuccessStatusCode) return new HttpResponseModel<T>(new T(), response.StatusCode, false);

        var responseContent = await response.Content.ReadAsStringAsync();
        var deserializedData = JsonSerializer.Deserialize<T>(responseContent, _serializerOptions) ?? new T();

        return new HttpResponseModel<T>(deserializedData, response.StatusCode, true);
    }
}