using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace ShkedGroupsService.DAL.Models;

public class Schedule
{
    [BsonId]
    [JsonIgnore]
    public string Id { get; set; }
    public string GroupName { get; set; }

    public Weekday[] Week { get; set; } = new Weekday[7];
    public Schedule()
    {
        for (int i = 0; i < 7; i++)
        {
            Week[i] = new Weekday() { DayNumber = i + 1, DaysSchedules = new HashSet<DailySchedule>() };
        }
    }
}