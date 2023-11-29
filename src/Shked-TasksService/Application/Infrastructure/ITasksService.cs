using ShkedTasksService.Application.DTO;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.Application.Infrastructure;

public interface ITasksService
{
    Task<IEnumerable<Attachment>?> CreateTaskAsync(TaskDTO taskDto, IFormFileCollection formFileCollection);
    Task<IEnumerable<TaskDTO>> GetTasksAsync(string userId);
    Task<bool> UploadTemporaryFile(IFormFile file, IFormFile miniature, string userId);
    Task<bool> DeleteTemporaryFile(string filePath);
    Task<IEnumerable<AttachmentDto>> GetTemporaryFiles(string userId);
}