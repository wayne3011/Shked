using MongoDB.Driver;
using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.DAL.Infrastructure;
/// <summary>
/// Предоставляет контекст БД с учебными группами
/// </summary>
public interface IScheduleDbContext
{
    /// <summary>
    /// Коллекция учебных групп
    /// </summary>
    IMongoCollection<Schedule> Schedules { get; }
}