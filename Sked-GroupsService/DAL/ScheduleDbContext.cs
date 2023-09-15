using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SkedGroupsService.Application.Models;
using SkedGroupsService.DAL.DbContext.Options;
using SkedGroupsService.DAL.Infrastructure;

namespace SkedGroupsService.DAL;

public class ScheduleDbContext : IScheduleDbContext
{
    public IMongoCollection<Schedule> Schedules { get; }

    public ScheduleDbContext(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        Schedules = database.GetCollection<Schedule>(options.Value.CollectionName);
    }
}