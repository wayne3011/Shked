using ShkedGroupsService.Application.DTO;
using ShkedGroupsService.Application.DTO.ScheduleDTO;

namespace ShkedGroupsService.Application.Infrastructure;

public interface IGroupsService
{
    Task<ScheduleDTO> GetGroupSchedule(string groupName);
    Task<GroupNameValidationResult> FormatGroupName(string groupName);
}