namespace SkedScheduleParser.Application.Models;

public class Schedule
{
    public string GroupName { get; set; }
    public Schedule()
    {
        for (int i = 1; i <= 7; i++)
        {
            Week.Add(new Weekday() { DayNumber = i});
        }
    }
    public List<Weekday> Week { get; } = new List<Weekday>();

}