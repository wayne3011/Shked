using ShkedTasksService.Application.DTO;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.Application.Infrastructure;

public interface ITaskAttachmentsStorageApi
{
    Task<bool> UploadTemporaryFile(IFormFile file, IFormFile thumbnail, string userId);
    Task<bool> DeleteTemporaryFile(string fileName, string userId);
    Task<IEnumerable<AttachmentDto>?> MoveFilesToPermanentAsync(string userId, string taskId);
    Task<IEnumerable<AttachmentDto>?> GetListOfTemporaryFile(string userId);
    Task<FileDTO?> GetTemporaryThumbnailAsync(string filename, string userId);
    Task<FileDTO?> GetTemporaryFileAsync(string filename, string userId);
    Task<FileDTO?> GetPermanentThumbnail(string filename, string taskId);
    Task<FileDTO?> GetPermanentFile(string filename, string taskId);
    Task<bool> DeletePermanentFile(string filename, string taskId);
}