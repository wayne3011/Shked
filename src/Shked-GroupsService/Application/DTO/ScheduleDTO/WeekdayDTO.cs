namespace ShkedGroupsService.Application.DTO.ScheduleDTO;

public class WeekdayDTO
{
    public HashSet<DAL.Models.DailySchedule> DaysSchedules { get; set; } = new HashSet<DAL.Models.DailySchedule>();
    public int DayNumber { get; set; } = 0;
}