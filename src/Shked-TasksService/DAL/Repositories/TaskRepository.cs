using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ShkedTasksService.Application.DTO;
using ShkedTasksService.DAL.Entities;
using ShkedTasksService.DAL.Infrastructure;

namespace ShkedTasksService.DAL.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ITaskDbContext _dbContext;
    public TaskRepository(ITaskDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> CreateAsync(TaskEntity newTask)
    {
        try
        {
            await _dbContext.Tasks.InsertOneAsync(newTask);
        }
        catch (TimeoutException e)
        {
            return false;
        }
        return true;
    }

    public async Task<IEnumerable<TaskEntity>> GetActualTasks(string groupName, string userId)
    {
        return (await _dbContext.Tasks.FindAsync(task => 
            task.GroupName == groupName 
            && ((task.UserID == userId && !task.IsPublic) || task.IsPublic))).ToList();
    }

    public async Task<TaskEntity?> FindAsync(string taskId)
    {
        var cursor = (await _dbContext.Tasks.FindAsync(x => x.Id == taskId)).ToList();
        return cursor.Any() ? cursor.First() : null;
    }

    public async Task<bool> DeleteAsync(string taskId)
    {
        var result = await _dbContext.Tasks.DeleteOneAsync(x => x.Id == taskId);
        return result.DeletedCount == 1;
    }
}
