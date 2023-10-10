using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.Application.Infrastructure;

public interface IScheduleApi
{
    Task<Schedule> GetScheduleAsync(string groupName);
}