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
}