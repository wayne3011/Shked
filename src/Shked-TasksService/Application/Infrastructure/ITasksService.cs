using ShkedTasksService.Application.DTO;

namespace ShkedTasksService.Application.Infrastructure;

public interface ITasksService
{
    Task<IEnumerable<string>?> CreateTaskAsync(TaskDTO taskDto, IFormFileCollection formFileCollection);
    Task<IEnumerable<TaskDTO>> GetTasksAsync(string userId);
}