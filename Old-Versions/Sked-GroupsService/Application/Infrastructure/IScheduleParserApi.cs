using SkedGroupsService.Application.Models;
using SkedScheduleParser.Application.Models;

namespace SkedGroupsService.Application.Infrastructure;

public interface IScheduleParserApi
{
    Task<bool> GetGroupSchedule(ParsingApplication parsingApplication);
    Task<GroupNameValidationResult?> FormatGroupName(string groupName);
}