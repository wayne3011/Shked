using AutoMapper;
using ShkedGroupsService.Application.DTO;
using ShkedGroupsService.Application.DTO.ScheduleDTO;
using ShkedGroupsService.Application.Infrastructure;
using ShkedGroupsService.DAL.Infrastructure;
using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.Application.Services;
/// <summary>
/// Представляет реализацию сервиса <see cref="IGroupsService"/> бизнес-логики, связанной с учебными группами
/// </summary>
public class GroupsService : IGroupsService
{
    private readonly IScheduleApi _scheduleApi;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GroupsService> _logger;

    public GroupsService(IScheduleApi scheduleApi, IScheduleRepository scheduleRepository, IMapper mapper, ILogger<GroupsService> logger)
    {
        _scheduleApi = scheduleApi;
        _scheduleRepository = scheduleRepository;
        _mapper = mapper;
        _logger = logger;
    }
    

    public async Task<ScheduleDTO?> GetGroupSchedule(string groupName)
    {
        Schedule? schedule = null;
        schedule = await _scheduleRepository.GetAsync(groupName);
        if (schedule == null)
        {
            schedule = await _scheduleApi.GetScheduleAsync(groupName);
            if (schedule != null)
            {
                schedule.Id = Guid.NewGuid().ToString();
                await _scheduleRepository.CreateAsync(schedule);
            }
        }
        
        return schedule != null ? _mapper.Map<Schedule,ScheduleDTO>(schedule) : null;
    }
}