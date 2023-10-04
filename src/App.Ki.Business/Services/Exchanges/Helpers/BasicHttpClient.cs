using System.Net.Http.Headers;
using System.Text;
using App.Ki.Business.Extensions;
using App.Ki.Commons.Models;
using Microsoft.Extensions.Logging;

namespace App.Ki.Business.Services.Exchanges.Helpers;

internal class BasicHttpClient
{
    private static readonly MediaTypeWithQualityHeaderValue JsonMedia = new("application/json");
    private readonly HttpClient _client;
    private readonly ILogger _logger;

    protected BasicHttpClient(HttpClient client, ILogger logger)
    {
        _client = client;
        _logger = logger;
    }

    protected Task<AppResult<T>> Get<T>(
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return Call<T>(HttpMethod.Get, null, uri, headers, token);
    }

    protected Task<AppResult<T>> GetForm<T>(
        string uri,
        Dictionary<string, object> formData = null,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return CallForm<T>(HttpMethod.Get, uri, formData, headers, token);
    }

    protected Task<AppResult<T>> Post<T>(
        object body,
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return Call<T>(HttpMethod.Post, body, uri, headers, token);
    }

    protected Task<AppResult<T>> Patch<T>(
        object body,
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return Call<T>(HttpMethod.Patch, body, uri, headers, token);
    }

    protected Task<AppResult<T>> PostForm<T>(
        string uri,
        Dictionary<string, object> formData = null,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        return CallForm<T>(HttpMethod.Post, uri, formData, headers, token);
    }

    private async Task<AppResult<T>> Call<T>(
        HttpMethod method,
        object body,
        string uri,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        using var request = PrepareRequest(method, uri, body, headers);
        return await Call<T>(request, token);
    }

    private async Task<AppResult<T>> CallForm<T>(
        HttpMethod method,
        string uri,
        Dictionary<string, object> formData,
        Dictionary<string, string> headers = null,
        CancellationToken token = default)
    {
        using var request = PrepareFormRequest(method, uri, formData, headers);
        return await Call<T>(request, token);
    }

    private async Task<AppResult<T>> Call<T>(HttpRequestMessage request, CancellationToken token = default)
    {
        using var response = await _client.SendAsync(request, token);
        var content = await response.Content.ReadAsStringAsync(token);
        
        if (!Equals(response.Content.Headers.ContentType, JsonMedia))
            return AppResult<T>.Bad("Invalid http response");
        
        var data = string.IsNullOrWhiteSpace(content) || content == "[]" ? default : content.FromJson<T>();
        if (response.IsSuccessStatusCode)
        {
            _logger.LogDebug("Response from {Uri}: {Data} {@Headers}", request.RequestUri, content, response.Headers);
            return AppResult<T>.Ok(data);
        }

        _logger.LogWarning("{Uri} call failed: {Response} {Code}", request.RequestUri, content, response.StatusCode);
        return new AppResult<T>
        {
            Data = data,
            StatusCode = (int)response.StatusCode,
            Message = $"External service call failed: status code {response.StatusCode}"
        };
    }

    private static HttpRequestMessage PrepareRequest(
        HttpMethod method,
        string uri,
        object body = null,
        Dictionary<string, string> headers = null)
    {
        var jsonBody = body?.ToJson();
        var request = new HttpRequestMessage(method, uri);
        if (headers != null)
            foreach (var keyValuePair in headers)
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);

        request.Headers.Accept.Add(JsonMedia);

        if (body != null)
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        return request;
    }

    private static HttpRequestMessage PrepareFormRequest(
        HttpMethod method,
        string uri,
        Dictionary<string, object> formData,
        Dictionary<string, string> headers = null)
    {
        var request = new HttpRequestMessage(method, uri);
        if (headers != null)
            foreach (var keyValuePair in headers)
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);

        request.Headers.Accept.Add(JsonMedia);
        if (formData is null) return request;

        var formContent = new MultipartFormDataContent();
        foreach (var (key, value) in formData)
            formContent.Add(new StringContent(value?.ToString() ?? string.Empty, Encoding.UTF8), key);

        request.Content = formContent;
        return request;
    }
}