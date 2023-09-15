using SkedGroupsService.Application.Models;

namespace SkedScheduleParser.Application.Models;

public class ParsingResponse
{
    public Schedule NewSchedule { get; set; }
    public string ClientID { get; set; }
}