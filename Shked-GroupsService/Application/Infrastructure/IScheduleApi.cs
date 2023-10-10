using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.Application.Infrastructure;

public interface IScheduleApi
{
    Task<Schedule> GetSchedule(string groupName);
}