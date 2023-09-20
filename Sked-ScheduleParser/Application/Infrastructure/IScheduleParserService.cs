using SkedScheduleParser.Application.Models;

namespace SkedScheduleParser.Application.Infrastructure;

public interface IScheduleParserService
{
    Task<Schedule> GetGroupScheduleAsync(string groupName);
    Task<GroupNameValidationResult> FormatGroupNameAsync(string groupName);
}