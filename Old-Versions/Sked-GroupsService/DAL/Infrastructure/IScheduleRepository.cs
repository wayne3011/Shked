using SkedGroupsService.Application.Models;
using SkedGroupsService.DAL.Models;

namespace SkedGroupsService.DAL.Infrastructure;

public interface IScheduleRepository
{
    Task<Schedule?> GetAsync(string groupName);
    Task CreateAsync(Schedule newSchedule);
    Task<bool> UpdateAsync(Schedule schedule);
}