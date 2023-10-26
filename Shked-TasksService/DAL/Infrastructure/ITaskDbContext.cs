using MongoDB.Driver;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.DAL.Infrastructure;

public interface ITaskDbContext
{
    IMongoCollection<TaskEntity> Tasks { get; }
}