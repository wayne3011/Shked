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
    private readonly IUsersApi _usersApi;

    public TasksService(ITaskRepository taskRepository, IMapper mapper, ITaskAttachmentsStorageApi attachmentsStorage, IUsersApi usersApi)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _attachmentsStorage = attachmentsStorage;
        _usersApi = usersApi;
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

    public async Task<IEnumerable<TaskDTO>> GetTasksAsync(string userId)
    {
        var user = await _usersApi.GetById(userId);
        if (user == null) return new List<TaskDTO>();
        var tasksEntity = await _taskRepository.FindAsync(task => task.GroupName == user.Group && ((task.UserID == user.Id && !task.IsPublic) || task.IsPublic));
        return _mapper.Map<IEnumerable<TaskEntity>, IEnumerable<TaskDTO>>(tasksEntity);
    }
}