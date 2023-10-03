using MongoDB.Driver;
using SkedGroupsService.Application.Models;
using SkedGroupsService.DAL.Models;

namespace SkedGroupsService.DAL.Infrastructure;

public interface IScheduleDbContext
{
    IMongoCollection<Schedule> Schedules { get; }
}