using MongoDB.Driver;
using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.DAL.Infrastructure;

public interface IScheduleDbContext
{
    IMongoCollection<Schedule> Schedules { get; }
}