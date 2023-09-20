namespace SkedScheduleParser.Application.Models;

public class Weekday
{
    public HashSet<DailySchedule> DaysSchedules { get; set; } = new HashSet<DailySchedule>();
    public int DayNumber { get; set; } = 0;
}