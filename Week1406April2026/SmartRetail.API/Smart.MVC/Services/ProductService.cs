using Smart.MVC.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace Smart.MVC.Services
{
    public class ProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("http://localhost:5000/");
        }

        public async Task<List<Product>> GetProducts()
        {
            try
            {
                var products = await _http.GetFromJsonAsync<List<Product>>("api/Product");
                return products ?? new List<Product>();
            }
            catch
            {
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProduct(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<Product>($"api/Product/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateProduct(Product product)
        {
            var resp = await _http.PostAsJsonAsync("api/Product", product);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> CreateProductWithImage(string name, decimal price, IFormFile imageFile)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(name), "name");
            content.Add(new StringContent(price.ToString()), "price");
            
            if (imageFile != null && imageFile.Length > 0)
            {
                var streamContent = new StreamContent(imageFile.OpenReadStream());
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
                content.Add(streamContent, "file", imageFile.FileName);
            }

            var resp = await _http.PostAsync("api/Product", content);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var resp = await _http.PutAsJsonAsync($"api/Product/{product.Id}", product);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var resp = await _http.DeleteAsync($"api/Product/{id}");
            return resp.IsSuccessStatusCode;
        }
    }
}