namespace SkedScheduleParser.Application.Models;

public class Schedule
{
    public string GroupName { get; set; }
    public Weekday[] Week { get; } = new Weekday[7];
    public Schedule()
    {
        for (int i = 0; i < 7; i++)
        {
            Week[i] = new Weekday() { DayNumber = i + 1, DaysSchedules = new HashSet<DailySchedule>() };
        }
    }

}