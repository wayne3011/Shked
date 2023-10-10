using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using ShkedGroupsService.Application.Infrastructure;
using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.Application.Sources;

public class ScheduleAPI : IScheduleApi
{
    private IOptions<ScheduleAPIOptions> _scheduleApiOptions;

    public ScheduleAPI(IOptions<ScheduleAPIOptions> scheduleApiOptions)
    {
        _scheduleApiOptions = scheduleApiOptions;
    }
    public Task<Schedule> GetSchedule(string groupName)
    {
        Uri scheduleUri;

            scheduleUri = new Uri(_scheduleApiOptions.Value.ScheduleApiUrl +
                                  _scheduleApiOptions.Value.SchedulesCollection + encoder.ComputeHash(groupName.ToUpper()));
        
    }

    private string MD5Hash(string value)
    {
        using (var encoder = MD5.Create())
        {
            return 
        }
    }
}