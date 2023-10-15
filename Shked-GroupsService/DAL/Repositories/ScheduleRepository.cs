using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using ShkedGroupsService.DAL.Models;
using ShkedGroupsService.DAL.Infrastructure;

namespace ShkedGroupsService.DAL.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly IScheduleDbContext _context;
    private readonly ILogger<ScheduleRepository> _logger;
    public ScheduleRepository(IScheduleDbContext context, ILogger<ScheduleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<Schedule?> GetAsync(string groupName)
    {
        IAsyncCursor<Schedule>? cursor = null;
        try
        {
            cursor = await _context.Schedules.FindAsync(x => x.GroupName == groupName);
        }
        catch (TimeoutException e)
        {
            _logger.LogError("Failed database connection.");
        }
        return cursor == null ? null : (await cursor.ToListAsync()).FirstOrDefault();
    }

    public async Task<bool> CreateAsync(Schedule newSchedule)
    {
        try
        {
            await _context.Schedules.InsertOneAsync(newSchedule);
        }
        catch (TimeoutException e)
        {
            _logger.LogError("Failed to save new schedule to database.");
            return false;
        }
        return true;
    }

    public async Task<bool> UpdateAsync(Schedule schedule)
    {
        var result =await _context.Schedules.ReplaceOneAsync(x => x.GroupName == schedule.GroupName, schedule);
        return result.IsAcknowledged && result.ModifiedCount == 1;
    }
}