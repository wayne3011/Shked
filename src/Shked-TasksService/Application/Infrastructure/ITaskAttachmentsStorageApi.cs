using ShkedTasksService.Application.DTO;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.Application.Infrastructure;

public interface ITaskAttachmentsStorageApi
{
    Task<bool> UploadTemporaryFile(IFormFile file, IFormFile thumbnail, string userId);
    Task<bool> DeleteTemporaryFile(string fileName, string userId);
    Task<IEnumerable<AttachmentDto>?> MoveFilesToPermanentAsync(string userId, string taskId);
}