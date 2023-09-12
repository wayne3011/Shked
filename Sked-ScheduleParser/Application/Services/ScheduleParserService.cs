using SkedScheduleParser.Application.Infrastructure;
using SkedScheduleParser.Application.Models;

namespace SkedScheduleParser.Application.Services;

public class ScheduleParserService : IScheduleParserService
{
    public async Task<Schedule> GetGroupScheduleAsync(string groupName)
    {
        //await Task.Delay(1000);
        return new Schedule() { GroupName = "М3О-325Бк-21" };
    }

    public Task<string?> FormatGroupNameAsync(string groupName)
    {
        throw new NotImplementedException();
    }
}