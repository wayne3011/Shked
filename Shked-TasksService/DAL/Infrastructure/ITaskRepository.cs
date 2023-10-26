using ShkedTasksService.Application.DTO;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.DAL.Infrastructure;

public interface ITaskRepository
{
    Task<bool> CreateAsync(TaskEntity newTask);
    Task<IEnumerable<TaskEntity>> FindAsync(Func<TaskEntity, bool> selector);
}