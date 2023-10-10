using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.DAL.Infrastructure;

public interface IScheduleRepository
{
    Task<Schedule?> GetAsync(string groupName);
    Task CreateAsync(Schedule newSchedule);
    Task<bool> UpdateAsync(Schedule schedule);
}