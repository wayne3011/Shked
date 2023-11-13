using AutoMapper;
using ShkedTasksService.Application.DTO;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.Application.Mapper;

public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<TaskDTO, TaskEntity>();
        CreateMap<TaskEntity, TaskDTO>();
    }
}