namespace AirsoftEvents.Persistance.Interface;

public interface IFieldImageRepo
{
    Task<string> UploadAsync(byte[] content, string blobName, string contentType);
}
