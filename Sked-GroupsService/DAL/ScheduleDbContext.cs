using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SkedGroupsService.Application.Models;
using SkedGroupsService.DAL.DbContext.Options;
using SkedGroupsService.DAL.Infrastructure;
using SkedGroupsService.DAL.Models;

namespace SkedGroupsService.DAL;

public class ScheduleDbContext : IScheduleDbContext
{
    public IMongoCollection<Schedule> Schedules { get; }
    public ScheduleDbContext(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        // client.Settings.SocketTimeout = TimeSpan.FromSeconds(10);
        // client.Settings.ConnectTimeout = TimeSpan.FromSeconds(10);
        // client.Settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
        var database = client.GetDatabase(options.Value.DatabaseName);
        Schedules = database.GetCollection<Schedule>(options.Value.CollectionName);
    }
}