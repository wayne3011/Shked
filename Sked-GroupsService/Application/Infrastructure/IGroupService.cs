namespace SkedGroupsService.Application.Infrastructure;

public interface IGroupService
{
    Task<bool> RequestGroupSchedule(string groupName);
    Task<string?> FormatGroupName(string groupName);
}