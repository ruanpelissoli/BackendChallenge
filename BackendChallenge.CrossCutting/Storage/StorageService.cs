using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace BackendChallenge.CrossCutting.Storage;

public interface IStorageService
{
    Task<string> UploadImageAsync(byte[] imageBytes, string blobName);
}

public class StorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public StorageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Storage");
        _containerName = "cnhimages";
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<string> UploadImageAsync(byte[] imageBytes, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(blobName);

        using (var stream = new MemoryStream(imageBytes))
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        return blobClient.Uri.ToString();
    }
}
