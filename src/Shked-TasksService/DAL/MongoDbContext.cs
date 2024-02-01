using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ShkedTasksService.DAL.DbContext.Options;
using ShkedTasksService.DAL.Entities;
using ShkedTasksService.DAL.Infrastructure;

namespace ShkedTasksService.DAL;

public class MongoDbContext : ITaskDbContext
{
    public MongoDbContext(IOptions<MongoOptions> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        Tasks = database.GetCollection<TaskEntity>(settings.Value.CollectionName);
    }
    public IMongoCollection<TaskEntity> Tasks { get; }
}