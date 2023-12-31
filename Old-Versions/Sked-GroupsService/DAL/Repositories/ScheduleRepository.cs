﻿using MongoDB.Driver;
using SkedGroupsService.Application.Models;
using SkedGroupsService.DAL.Infrastructure;
using SkedGroupsService.DAL.Models;

namespace SkedGroupsService.DAL.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly IScheduleDbContext _context;

    public ScheduleRepository(IScheduleDbContext context)
    {
        _context = context;
    }
    public async Task<Schedule?> GetAsync(string groupName)
    {
        var cursor = await _context.Schedules.FindAsync(x => x.GroupName == groupName);
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