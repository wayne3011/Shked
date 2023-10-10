using System.Text.Json.Serialization;

namespace ShkedGroupsService.Application.DTO.ScheduleDTO;

public class ScheduleDTO
{
    [JsonPropertyName("groupName")] public string GroupName { get; set; } = null!;

    public WeekdayDTO[] Week { get; set; } = new WeekdayDTO[7];

}