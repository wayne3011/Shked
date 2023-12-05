using ShkedStorageService.Application.DTO;

namespace ShkedStorageService.Application.Infrastructure;

public interface ITaskAttachmentsService
{
    Task<IEnumerable<string>> CreateAttachmentsAsync(IFormFileCollection fileStream, string taskId);
    Task<CreationResult?> CreateTemporaryFileAsync(IFormFile miniature, IFormFile file, string userId);
    // Task<FileDTO?> GetFileThumbnail(string fileName, string userId);
    Task<bool> MoveToPermanentFiles(string userId, string taskId);
    Task<FileDTO> GetAttachmentAsync(string folder, string fileName);
    Task<FileDTO?> GetTemporaryThumbnail(string userId, string fileName);
    Task<FileDTO?> GetTemporaryFile(string userId, string fileName);
}