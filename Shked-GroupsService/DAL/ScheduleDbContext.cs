using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ShkedGroupsService.DAL.Infrastructure;
using ShkedGroupsService.DAL.Models;
using ShkedGroupsService.DAL.Options;


namespace ShkedGroupsService.DAL;
/// <summary>
/// Представляет контекст БД учебных групп <see cref="IScheduleDbContext"/>
/// </summary>
public class ScheduleDbContext : IScheduleDbContext
{
    public IMongoCollection<Schedule> Schedules { get; }
    public IOptions<MongoOptions> Options { get; }
    public ScheduleDbContext(IOptions<MongoOptions> options)
    {
        Options = options;
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);
        Schedules = database.GetCollection<Schedule>(options.Value.CollectionName);
    }
}