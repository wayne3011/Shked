using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.DAL.Infrastructure;

public interface ITaskRepository
{
    Task<bool> CreateAsync(TaskEntity newTask);
}