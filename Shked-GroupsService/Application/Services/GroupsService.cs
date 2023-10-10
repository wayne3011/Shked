using ShkedGroupsService.Application.DTO;
using ShkedGroupsService.Application.DTO.ScheduleDTO;
using ShkedGroupsService.Application.Infrastructure;

namespace ShkedGroupsService.Application.Services;

public class GroupsService : IGroupsService
{
    public Task<ScheduleDTO> GetGroupSchedule(string groupName)
    {
        throw new NotImplementedException();
    }

    public Task<GroupNameValidationResult> FormatGroupName(string groupName)
    {
        throw new NotImplementedException();
    }
}