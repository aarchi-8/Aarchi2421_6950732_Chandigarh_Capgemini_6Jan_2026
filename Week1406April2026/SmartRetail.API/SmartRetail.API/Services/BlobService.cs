using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;

public class BlobService
{
    private readonly string _connectionString;
    private readonly string _containerName = "images";

    public BlobService(IConfiguration config)
    {
        var blobConn = config["BlobConnection"];
        _connectionString = blobConn;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var client = new BlobContainerClient(_connectionString, _containerName);
        var blob = client.GetBlobClient(file.FileName);

        using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, true);

        return blob.Uri.ToString();
    }
}
