using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
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
    public async Task<IEnumerable<Attachment>?> CreateTaskAsync(TaskDTO taskDto, IFormFileCollection formFileCollection)
    {
        var newTask = _mapper.Map<TaskDTO, TaskEntity>(taskDto);
        var user = await _usersApi.GetById(taskDto.UserID);
        if (user == null) return null;
        newTask.GroupName = user.Group;
        newTask.Id = Guid.NewGuid().ToString();
        var attachments = await _attachmentsStorage.CreateAttachments(formFileCollection, newTask.Id);
        if (attachments == null)
        {
            return null;
        }
        newTask.Attachments = attachments;
        try
        { 
            await _taskRepository.CreateAsync(newTask);
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
        var tasksEntity = await _taskRepository.GetActualTasks(user.Group,user.Id);
        return _mapper.Map<IEnumerable<TaskEntity>, IEnumerable<TaskDTO>>(tasksEntity);
    }

    public Task<bool> UploadTemporaryFile(IFormFile file, IFormFile miniature, string userId)
    {
        
    }

    public Task<bool> DeleteTemporaryFile(string filePath)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AttachmentDto>> GetTemporaryFiles(string userId)
    {
        throw new NotImplementedException();
    }
}