using ShkedTasksService.Application.DTO;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.Application.Infrastructure;

public interface ITasksService
{
    Task<IEnumerable<TaskDTO>> GetTasksAsync(string userId);
    Task<TaskDTO?> CreateTaskAsync(string userId, TaskDTO taskDto); 
    Task<bool> UploadTemporaryFileAsync(IFormFile file, IFormFile miniature, string userId);
    Task<bool> DeleteTemporaryFileAsync(string fileName, string userId);
    Task<AttachmentDto> GetTemporaryThumbnailAsync(string fileName);
    Task<AttachmentDto> GetTemporaryFileAsync(string userId);
}