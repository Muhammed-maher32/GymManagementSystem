using GymSystem.BLL.Services.Common;

namespace GymSystem.BLL.Services.Attachment;

public interface IAttachmentService
{
    Task<Result<string>> UploadAsync(Stream fileStream, string fileName, string folderName,
        CancellationToken ct);
}
