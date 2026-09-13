using System.Security.Cryptography;

namespace SmartX.Api.Infrastructure.FileStorage;

public class EncryptedFileStorageService : IFileStorageService
{
    private static readonly string[] AllowedExtensions = { ".txt", ".log", ".json", ".jpg", ".jpeg", ".png", ".pdf" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    private readonly string _rootPath;
    private readonly byte[] _key;

    public EncryptedFileStorageService(IConfiguration config, IWebHostEnvironment env)
    {
        _rootPath = Path.Combine(env.ContentRootPath, "wwwroot", "uploads");
        var configuredKey = config["FileEncryption:Key"] ?? "SmartX-Dev-Key-Replace-In-Prod!!";
        _key = System.Text.Encoding.UTF8.GetBytes(configuredKey.PadRight(32)[..32]);
    }

    public async Task<string> SaveEncryptedAsync(string sensorMac, string fileName, Stream content)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException($"File type '{ext}' is not permitted.");

        if (content.Length > MaxFileSizeBytes)
            throw new InvalidOperationException("File exceeds the 10MB upload limit.");

        var dir = Path.Combine(_rootPath, sensorMac);
        Directory.CreateDirectory(dir);
        var savePath = Path.Combine(dir, fileName + ".enc");

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        await using var output = File.Create(savePath);
        await output.WriteAsync(aes.IV);
        await using var cryptoStream = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await content.CopyToAsync(cryptoStream);

        return savePath;
    }

    public Stream OpenDecryptedStream(string storedPath)
    {
        var fileStream = File.OpenRead(storedPath);
        var iv = new byte[16];
        fileStream.ReadExactly(iv, 0, iv.Length);

        var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = iv;

        return new CryptoStream(fileStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
    }
}