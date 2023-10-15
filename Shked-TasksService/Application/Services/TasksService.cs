using AutoMapper;
using ShkedTasksService.Application.DTO;
using ShkedTasksService.Application.Infrastructure;
using ShkedTasksService.DAL.Entities;
using ShkedTasksService.DAL.Infrastructure;

namespace ShkedTasksService.Application.Services;

public class TasksService : ITasksService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskAttachmentsStorageApi _attachmentsStorage;
    private readonly IMapper _mapper;
    public TasksService(ITaskRepository taskRepository, IMapper mapper, ITaskAttachmentsStorageApi attachmentsStorage)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _attachmentsStorage = attachmentsStorage;
    }
    public async Task<IEnumerable<string>?> CreateTaskAsync(TaskDTO taskDto, IFormFileCollection formFileCollection)
    {
        var newTask = _mapper.Map<TaskDTO, TaskEntity>(taskDto);
        newTask.Id = Guid.NewGuid().ToString();
        try
        {
            await _taskRepository.CreateAsync(newTask);
            newTask.Attachments = await _attachmentsStorage.CreateAttachments(formFileCollection, newTask.Id);
        }
        catch (TimeoutException e)
        {
            return null;
        }

        return newTask.Attachments;
    }
    
}