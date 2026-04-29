using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BookStore.Shared;
using BookStore.Web.Models;
namespace BookStore.Web.Services;

public class ApiService
{
    private readonly HttpClient _http; private readonly IHttpContextAccessor _accessor;
    private static readonly JsonSerializerOptions J = new() { PropertyNameCaseInsensitive = true };
    public ApiService(HttpClient http, IHttpContextAccessor accessor) { _http = http; _accessor = accessor; }
    private void Attach() { var t = _accessor.HttpContext?.Session.GetString("JwtToken"); if (!string.IsNullOrEmpty(t)) _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", t); }
    public async Task<T?> GetAsync<T>(string url) { Attach(); var r = await _http.GetAsync(url); if (!r.IsSuccessStatusCode) return default; var w = JsonSerializer.Deserialize<ApiResponse<T>>(await r.Content.ReadAsStringAsync(), J); return w is { Success: true } ? w.Data : default; }
    public async Task<ApiResponse> PostAsync<T>(string url, T data) { Attach(); var r = await _http.PostAsync(url, new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")); return JsonSerializer.Deserialize<ApiResponse>(await r.Content.ReadAsStringAsync(), J) ?? ApiResponse.Fail("Error."); }
    public async Task<ApiResponse<BookStore.Application.DTOs.BookDto>> PostBookWithImageAsync(string url, BookFormModel model)
    {
        Attach();
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(model.Title), "Title");
        content.Add(new StringContent(model.ISBN), "ISBN");
        content.Add(new StringContent(model.Price.ToString()), "Price");
        content.Add(new StringContent(model.Stock.ToString()), "Stock");
        content.Add(new StringContent(model.CategoryId.ToString()), "CategoryId");
        content.Add(new StringContent(model.AuthorId.ToString()), "AuthorId");
        content.Add(new StringContent(model.PublisherId.ToString()), "PublisherId");
        if (model.ImageFile != null)
        {
            var fileContent = new StreamContent(model.ImageFile.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);
            content.Add(fileContent, "imageFile", model.ImageFile.FileName);
        }
        var r = await _http.PostAsync(url, content);
        return JsonSerializer.Deserialize<ApiResponse<BookStore.Application.DTOs.BookDto>>(await r.Content.ReadAsStringAsync(), J) ?? ApiResponse<BookStore.Application.DTOs.BookDto>.Fail("Error.");
    }
    public async Task<ApiResponse> PutAsync<T>(string url, T data) { Attach(); var r = await _http.PutAsync(url, new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")); return JsonSerializer.Deserialize<ApiResponse>(await r.Content.ReadAsStringAsync(), J) ?? ApiResponse.Fail("Error."); }
    public async Task<ApiResponse> PutBookWithImageAsync(string url, BookFormModel model)
    {
        Attach();
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(model.Title), "Title");
        content.Add(new StringContent(model.ISBN), "ISBN");
        content.Add(new StringContent(model.Price.ToString()), "Price");
        content.Add(new StringContent(model.Stock.ToString()), "Stock");
        content.Add(new StringContent(model.CategoryId.ToString()), "CategoryId");
        content.Add(new StringContent(model.AuthorId.ToString()), "AuthorId");
        content.Add(new StringContent(model.PublisherId.ToString()), "PublisherId");
        if (!string.IsNullOrEmpty(model.ImageUrl))
            content.Add(new StringContent(model.ImageUrl), "ImageUrl");
        if (model.ImageFile != null)
        {
            var fileContent = new StreamContent(model.ImageFile.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageFile.ContentType);
            content.Add(fileContent, "imageFile", model.ImageFile.FileName);
        }
        var r = await _http.PutAsync(url, content);
        return JsonSerializer.Deserialize<ApiResponse>(await r.Content.ReadAsStringAsync(), J) ?? ApiResponse.Fail("Error.");
    }
    public async Task<ApiResponse> PatchAsync(string url, string body) { Attach(); var req = new HttpRequestMessage(HttpMethod.Patch, url) { Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json") }; var r = await _http.SendAsync(req); return JsonSerializer.Deserialize<ApiResponse>(await r.Content.ReadAsStringAsync(), J) ?? ApiResponse.Fail("Error."); }
    public async Task<ApiResponse<TResponse>> PostWithResponseAsync<TRequest, TResponse>(string url, TRequest data) { Attach(); var r = await _http.PostAsync(url, new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")); return JsonSerializer.Deserialize<ApiResponse<TResponse>>(await r.Content.ReadAsStringAsync(), J) ?? ApiResponse<TResponse>.Fail("Error."); }
    public async Task<ApiResponse> DeleteAsync(string url)
    {
        Attach();
        try
        {
            var r = await _http.DeleteAsync(url);
            if (r.IsSuccessStatusCode)
            {
                var content = await r.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(content))
                    return ApiResponse.Ok("Deleted successfully.");
                try
                {
                    return JsonSerializer.Deserialize<ApiResponse>(content, J) ?? ApiResponse.Ok("Deleted successfully.");
                }
                catch
                {
                    return ApiResponse.Ok("Deleted successfully.");
                }
            }
            else
            {
                try
                {
                    var content = await r.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ApiResponse>(content, J) ?? ApiResponse.Fail($"Error: {(int)r.StatusCode}");
                }
                catch
                {
                    return ApiResponse.Fail($"Error: {(int)r.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"Request failed: {ex.Message}");
        }
    }
}