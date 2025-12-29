using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using AirsoftEvents.Persistance.Interface;

namespace AirsoftEvents.Persistance;

public class FieldImageStorageOptions
{
    public string ConnectionString { get; set; } = default!;
    public string Container { get; set; } = default!;
}

public class FieldImageRepo : IFieldImageRepo
{
    private readonly FieldImageStorageOptions _options;

    public FieldImageRepo(IOptions<FieldImageStorageOptions> options)
    {
        _options = options.Value;
    }

    private BlobClient GetBlobClient(string blobName)
    {
        var serviceClient = new BlobServiceClient(_options.ConnectionString);
        var containerClient = serviceClient.GetBlobContainerClient(_options.Container);
        return containerClient.GetBlobClient(blobName);
    }

    public async Task<string> UploadAsync(byte[] content, string blobName, string contentType)
    {
        var blob = GetBlobClient(blobName);

        await blob.UploadAsync(
            new BinaryData(content),
            new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            });

        return blob.Uri.ToString();
    }
}
