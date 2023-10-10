using AutoMapper;
using ShkedGroupsService.Application.DTO.ScheduleDTO;
using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.Application.Mapper;

public class ScheduleProfile : Profile
{
    public ScheduleProfile()
    {
        CreateMap<Schedule, ScheduleDTO>();
        CreateMap<Weekday, WeekdayDTO>();
        CreateMap<DailySchedule, DailyScheduleDTO>();
        CreateMap<Lesson, LessonDTO>();
    }
}