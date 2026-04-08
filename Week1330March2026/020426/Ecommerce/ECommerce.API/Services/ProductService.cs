using Microsoft.Extensions.Caching.Memory;

public class ProductService
{
    private readonly IMemoryCache _cache;

    public ProductService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<string> GetProducts()
    {
        if (_cache.TryGetValue("products", out string? cached))
            return cached!;

        var data = "Data from DB";

        _cache.Set("products", data, TimeSpan.FromMinutes(5));

        return await Task.FromResult(data);
    }
}