using MongoDB.Driver;
using ShkedGroupsService.DAL.Models;
using ShkedGroupsService.DAL.Infrastructure;

namespace ShkedGroupsService.DAL.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly IScheduleDbContext _context;

    public ScheduleRepository(IScheduleDbContext context)
    {
        _context = context;
    }
    public async Task<Schedule?> GetAsync(string groupName)
    {
        try
        {
            var cursor = await _context.Schedules.FindAsync(x => x.GroupName == groupName);
        }
        catch (TimeoutException e)
        {

        }
        return (await cursor.ToListAsync()).FirstOrDefault();
    }

    public async Task CreateAsync(Schedule newSchedule)
    {
        await _context.Schedules.InsertOneAsync(newSchedule);
    }

    public async Task<bool> UpdateAsync(Schedule schedule)
    {
        var result =await _context.Schedules.ReplaceOneAsync(x => x.GroupName == schedule.GroupName, schedule);
        return result.IsAcknowledged && result.ModifiedCount == 1;
    }
}