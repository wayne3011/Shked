using AutoMapper;
using ShkedGroupsService.Application.DTO;
using ShkedGroupsService.Application.DTO.ScheduleDTO;
using ShkedGroupsService.Application.Infrastructure;
using ShkedGroupsService.DAL.Infrastructure;
using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.Application.Services;

public class GroupsService : IGroupsService
{
    private readonly IScheduleApi _scheduleApi;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IMapper _mapper;

    public GroupsService(IScheduleApi scheduleApi, IScheduleRepository scheduleRepository, IMapper mapper)
    {
        _scheduleApi = scheduleApi;
        _scheduleRepository = scheduleRepository;
        _mapper = mapper;
    }
    

    public async Task<ScheduleDTO> GetGroupSchedule(string groupName)
    {
        var schedule = await _scheduleRepository.GetAsync(groupName);
        if (schedule == null)
        {
            schedule = await _scheduleApi.GetScheduleAsync(groupName);
            schedule.Id = Guid.NewGuid().ToString();
            await _scheduleRepository.CreateAsync(schedule);
        }
        
        return _mapper.Map<Schedule,ScheduleDTO>(schedule);
    }

    public Task<GroupNameValidationResult> FormatGroupName(string groupName)
    {
        throw new NotImplementedException();
    }
}