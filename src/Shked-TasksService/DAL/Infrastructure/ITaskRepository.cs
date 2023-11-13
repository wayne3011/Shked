using ShkedTasksService.Application.DTO;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.DAL.Infrastructure;

public interface ITaskRepository
{
    Task<bool> CreateAsync(TaskEntity newTask);
    Task<IEnumerable<TaskEntity>> GetActualTasks(string groupName, string userId);
    
}