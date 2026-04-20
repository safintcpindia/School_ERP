using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SchoolERP.Net.Models.Common;

namespace SchoolERP.Net.Services.Clients
{
    /// <summary>
    /// Serves as the foundational HTTP proxy layer for all Blazor/MVC frontend components.
    /// Intercepts raw JSON payloads, automatically unwrapping them into typed <c>ApiResponse&lt;T&gt;</c> structures
    /// and gracefully catching network-level timeouts or HTTP fault codes locally.
    /// </summary>
    public abstract class BaseApiClient
    {
        protected readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpContextAccessor = new HttpContextAccessor();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected async Task<ApiResponse<T>> GetAsync<T>(string url)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                AttachBearerToken(request);
                var response = await _httpClient.SendAsync(request);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.ErrorResponse($"Network Error: {ex.Message}");
            }
        }

        protected async Task<ApiResponse<T>> PostAsync<T>(string url, object data)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = JsonContent.Create(data)
                };
                AttachBearerToken(request);
                var response = await _httpClient.SendAsync(request);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.ErrorResponse($"Network Error: {ex.Message}");
            }
        }

        protected async Task<ApiResponse<T>> PutAsync<T>(string url, object data)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = JsonContent.Create(data)
                };
                AttachBearerToken(request);
                var response = await _httpClient.SendAsync(request);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.ErrorResponse($"Network Error: {ex.Message}");
            }
        }

        protected async Task<ApiResponse<T>> DeleteAsync<T>(string url)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, url);
                AttachBearerToken(request);
                var response = await _httpClient.SendAsync(request);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<T>.ErrorResponse($"Network Error: {ex.Message}");
            }
        }

        private void AttachBearerToken(HttpRequestMessage request)
        {
            var token = _httpContextAccessor.HttpContext?.Request?.Cookies["token"];
            if (string.IsNullOrWhiteSpace(token)) return;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// Sinks the raw native response content into memory.
        /// Identifies whether the JSON wraps a generic data structure or a standardized 'ApiResponse' object, 
        /// unpacking the Success flag naturally regardless of API divergence.
        /// </summary>
        private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    using (var jsonDoc = JsonDocument.Parse(content))
                    {
                        if (jsonDoc.RootElement.ValueKind == JsonValueKind.Object && 
                            (jsonDoc.RootElement.TryGetProperty("success", out _) || 
                             jsonDoc.RootElement.TryGetProperty("Success", out _)))
                        {
                            return JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
                        }
                    }
                    
                    // Fallback block: wraps raw arrays / primitives into standard GUI tracking structures mapping
                    var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                    return ApiResponse<T>.SuccessResponse(data);
                }
                catch
                {
                    try
                    {
                        var data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                        return ApiResponse<T>.SuccessResponse(data);
                    }
                    catch
                    {
                        return ApiResponse<T>.ErrorResponse($"API Error: {response.ReasonPhrase}", new List<string> { content });
                    }
                }
            }

            try
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
                if (errorResponse != null) return errorResponse;
            }
            catch { }

            return ApiResponse<T>.ErrorResponse($"API Error: {response.ReasonPhrase}", new List<string> { content });
        }
    }
}
