using ShkedTasksService.Application.DTO;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.Application.Infrastructure;

public interface ITasksService
{
    Task<IEnumerable<TaskDTO>> GetTasksAsync(string userId);
    Task<TaskDTO?> CreateTaskAsync(string userId, TaskDTO taskDto); 
    Task<bool> UploadTemporaryFileAsync(IFormFile file, IFormFile miniature, string userId);
    Task<bool> DeleteTemporaryFileAsync(string fileName, string userId);
    Task<FileDTO?> GetTemporaryThumbnailAsync(string fileName, string userId);
    Task<FileDTO?> GetTemporaryFileAsync(string fileName, string userId);
    Task<IEnumerable<AttachmentDto>?> GetListOfTemporaryFile(string userId);
    Task<FileDTO?> GetPermanentThumbnailAsync(string fileName, string taskId, string userId);
    Task<FileDTO?> GetPermanentFileAsync(string fileName, string taskId, string userId);

    Task<bool> DeleteTaskAsync(string taskId, string userId);
}