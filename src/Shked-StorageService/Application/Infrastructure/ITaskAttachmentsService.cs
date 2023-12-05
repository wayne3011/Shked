using ShkedStorageService.Application.DTO;

namespace ShkedStorageService.Application.Infrastructure;

public interface ITaskAttachmentsService
{
    Task<CreationResult?> CreateTemporaryFileAsync(IFormFile miniature, IFormFile file, string userId);
    Task<bool> MoveToPermanentFilesAsync(string userId, string taskId);
    Task<FileDTO?> GetTemporaryThumbnailAsync(string userId, string fileName);
    Task<FileDTO?> GetTemporaryFileAsync(string userId, string fileName);
    Task<bool> DeleteTemporaryFileAsync(string userId, string fileName);
    Task<FileDTO?> GetTaskAttachment(string taskId, string filename);
    Task<FileDTO?> GetTaskAttachmentThumbnail(string taskId, string fileName);
    Task<bool> DeletePermanentFileAsync(string taskId, string fileName);
}