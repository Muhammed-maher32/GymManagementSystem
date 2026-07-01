using GymSystem.BLL.Services.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GymSystem.BLL.Services.Attachment;

public class AttachmentService : IAttachmentService
{
    private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly ILogger<AttachmentService> _logger;
    private readonly IWebHostEnvironment _env;
    public AttachmentService(IWebHostEnvironment env, ILogger<AttachmentService> logger)
    {
        _env = env;
        _logger = logger;
    }

    public async Task<Result<string>> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct)
    {


        if (fileStream is null || !fileStream.CanRead)
            return Result<string>.Fail("Invalid file stream.");

        if (fileStream.Length == 0)
            return Result<string>.Fail("No file provided.");

        if (fileStream.Length > _maxFileSize)
        {
            _logger.LogWarning("Rejected upload: file too large ({Size} bytes).", fileStream.Length);
            return Result<string>.Fail("File too large.");
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Rejected upload: extension {Ext} not allowed.", extension);
            return Result<string>.Fail("Unsupported file.");
        }

        var folderPath = Path.Combine(_env.ContentRootPath, folderName);
        Directory.CreateDirectory(folderPath);

        //Unique foldername
        var storedFileName = $"{Guid.NewGuid()}{fileName}";
        var filePath = Path.Combine(folderPath, storedFileName);

        try
        {
            await using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await fileStream.CopyToAsync(fs, ct);
            return Result<string>.Ok(storedFileName);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName}.", fileName);
            return Result<string>.Fail("Failed to save file: {ex.message}");
        }

    }
}
