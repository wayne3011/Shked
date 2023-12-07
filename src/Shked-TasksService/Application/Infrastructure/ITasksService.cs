using ShkedTasksService.Application.DTO;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.Application.Infrastructure;

public interface ITasksService
{
    Task<IEnumerable<TaskDTO>> GetTasksAsync(string userId);
    Task<bool> UploadTemporaryFile(IFormFile file, IFormFile miniature, string userId);
    Task<bool> DeleteTemporaryFile(string fileName);
    Task<AttachmentDto> GetTemporaryThumbnail(string fileName);
    Task<AttachmentDto> GetTemporaryFile(string userId);
}