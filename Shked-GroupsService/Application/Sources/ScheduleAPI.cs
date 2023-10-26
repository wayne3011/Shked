using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using ShkedGroupsService.Application.Infrastructure;
using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.Application.Sources;
/// <summary>
/// Представляет реализацию надстройку над API ВУЗа для получения расписания <see cref="IScheduleApi"/>
/// </summary>
public class ScheduleAPI : IScheduleApi
{
    private readonly IOptions<ScheduleAPIOptions> _scheduleApiOptions;

    public ScheduleAPI(IOptions<ScheduleAPIOptions> scheduleApiOptions)
    {
        _scheduleApiOptions = scheduleApiOptions;
    }
    public async Task<Schedule?> GetScheduleAsync(string groupName)
    {
        var scheduleUri = new Uri(_scheduleApiOptions.Value.ScheduleApiUrl + 
                                  _scheduleApiOptions.Value.SchedulesCollection + 
                                  MD5Hash(groupName)+ ".json");
        using var httpClient = new HttpClient();
        var httpResponse = await httpClient.GetAsync(scheduleUri);
        string content = string.Empty;
        if (httpResponse.IsSuccessStatusCode)
        {
            content = await httpResponse.Content.ReadAsStringAsync();
        }
        else if (httpResponse.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        return ScheduleResponseDeserialize(content);
    }
    /// <summary>
    /// Десереалиазация расписания полученного с API ВУЗа
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private Schedule ScheduleResponseDeserialize(string content)
    {
        var dictionary = JsonSerializer.Deserialize<Dictionary<string,JsonElement>>(content);
        var schedule = new Schedule
        {
            GroupName = dictionary["group"].ToString()
        };

        foreach (var oneDay in dictionary.Where(x => x.Key != "group"))
        {
            DailyScheduleApiModel dailyScheduleApiModel = oneDay.Value.Deserialize<DailyScheduleApiModel>();
            DailySchedule dailySchedule = new DailySchedule();
            DateOnly date = DateOnly.FromDateTime(Convert.ToDateTime(oneDay.Key, new CultureInfo("ru-Ru")));
            string hashSum = string.Empty;
            foreach (var oneTime in dailyScheduleApiModel.pairs)
            {
                string startTime = oneTime.Key;
                foreach (var lesson in oneTime.Value)
                {
                    string subject = lesson.Key;
                    int ordinal = GetOrdinalFromStartTime(startTime);
                    string type = ClassType[lesson.Value.type.First().Key];
                    string location = string.Join("/",lesson.Value.room.Select(x => x.Value));
                    string lector = string.Join("/",lesson.Value.lector.Select(x => x.Value));
                    hashSum += subject + ordinal + type + lector + location;
                    Lesson newLesson = new Lesson()
                    {
                        Name = subject,
                        Location = location,
                        Lecturer = lector,
                        Ordinal = ordinal,
                        Type = type
                    };
                    dailySchedule.Classes.Add(newLesson);
                }
            }
            dailySchedule.HashSum = MD5Hash(hashSum);
            var weekday = schedule.Week[(int)date.DayOfWeek - 1];
            weekday.DaysSchedules.TryGetValue(dailySchedule, out var alreadyExist);
            if (alreadyExist != null)
            {
                alreadyExist.Dates.Add(date);
            }
            else
            {
                dailySchedule.Dates.Add(date);
                weekday.DaysSchedules.Add(dailySchedule);
            }
        }

        return schedule;
    }

    private string MD5Hash(string value)
    {
        using var encoder = MD5.Create();
        return Convert.ToHexString(encoder.ComputeHash(Encoding.UTF8.GetBytes(value))).ToLower();
    }
    private static int GetOrdinalFromStartTime(string time)
    {
        switch (time)
        {
            case "9:00:00":
                return 1;
            case "10:45:00":
                return 2;
            case "13:00:00":
                return 3;
            case "14:45:00":
                return 4;
            case "16:30:00":
                return 5;
            case "18:15:00":
                return 6;
            case "20:00:00":
                return 7;
            default:
                throw new Exception("Invalid class time");
        }
    }
    /// <summary>
    /// Сопоставление типов занятий
    /// </summary>
    private readonly Dictionary<string, string> ClassType = new Dictionary<string, string>()
    {
        { "ЛК", "lecture" },
        { "ПЗ", "practical" },
        { "ЛР", "laboratory" },
        { "Экзамен", "exam" },
    };

}