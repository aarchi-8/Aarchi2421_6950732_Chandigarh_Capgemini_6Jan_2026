using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BookStore.Application.Interfaces;
using Microsoft.Extensions.Configuration;
namespace BookStore.Infrastructure;

public class BlobService : IBlobService
{
    private readonly BlobContainerClient _container;
    public BlobService(IConfiguration config)
    {
        var connStr = config["AzureBlobStorage:ConnectionString"];
        var containerName = config["AzureBlobStorage:ContainerName"] ?? "book-images";
        if (!string.IsNullOrEmpty(connStr))
        {
            var client = new BlobServiceClient(connStr);
            _container = client.GetBlobContainerClient(containerName);
            _container.CreateIfNotExists(PublicAccessType.Blob);
        }
        else
        {
            _container = null!;
        }
    }
    public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
    {
        if (_container == null) return $"/images/{fileName}";
        var blobName = $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}";
        var blob = _container.GetBlobClient(blobName);
        await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = GetContentType(fileName) });
        return blob.Uri.ToString();
    }
    public async Task DeleteImageAsync(string imageUrl)
    {
        if (_container == null || string.IsNullOrEmpty(imageUrl)) return;
        var blobName = Path.GetFileName(new Uri(imageUrl).LocalPath);
        await _container.GetBlobClient(blobName).DeleteIfExistsAsync();
    }
    private static string GetContentType(string f) => Path.GetExtension(f).ToLower() switch { ".jpg" or ".jpeg" => "image/jpeg", ".png" => "image/png", ".webp" => "image/webp", _ => "application/octet-stream" };
}