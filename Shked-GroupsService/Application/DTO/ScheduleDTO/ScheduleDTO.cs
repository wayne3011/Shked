using System.Text.Json.Serialization;

namespace ShkedGroupsService.Application.DTO.ScheduleDTO;

public class ScheduleDTO
{
    [JsonPropertyName("groupName")] public string GroupName { get; set; } = null!;

    public List<WeekdayDTO> Week { get; set; } = new List<WeekdayDTO>();
    // public Schedule()
    // {
    //     for (int i = 0; i < 7; i++)
    //     {
    //         Week[i] = new Weekday() { DayNumber = i + 1, DaysSchedules = new HashSet<DailySchedule>() };
    //     }
    // }
}