namespace SmartX.Api.Infrastructure.FileStorage;

public interface IFileStorageService
{
    Task<string> SaveEncryptedAsync(string sensorMac, string fileName, Stream content);
}