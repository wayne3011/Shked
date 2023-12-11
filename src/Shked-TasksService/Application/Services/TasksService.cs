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

    public TasksService(ITaskRepository taskRepository, IMapper mapper, ITaskAttachmentsStorageApi attachmentsStorage,
        IUsersApi usersApi)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _attachmentsStorage = attachmentsStorage;
        _usersApi = usersApi;
    }
    public async Task<IEnumerable<TaskDTO>> GetTasksAsync(string userId)
    {
        var user = await _usersApi.GetById(userId);
        if (user == null) return new List<TaskDTO>();
        var tasksEntity = await _taskRepository.GetActualTasks(user.Group,user.Id);
        return _mapper.Map<IEnumerable<TaskEntity>, IEnumerable<TaskDTO>>(tasksEntity);
    }

    public async Task<TaskDTO?> CreateTaskAsync(string userId, TaskDTO taskDto)
    {
        var user = await _usersApi.GetById(userId);
        if (user == null) return null;
        
        string taskId = Guid.NewGuid().ToString();
        taskDto.Id = taskId;
        taskDto.UserID = userId;
        taskDto.GroupName = user.Group;
        
        var response = await _attachmentsStorage.MoveFilesToPermanentAsync(userId, taskId);
        if (response == null) return null;
        taskDto.Attachments = response;

        var result = await _taskRepository.CreateAsync(_mapper.Map<TaskEntity>(taskDto));
        if (result) return taskDto;
        return null;
    }

    public async Task<bool> UploadTemporaryFileAsync(IFormFile file, IFormFile miniature, string userId)
    {
        return await _attachmentsStorage.UploadTemporaryFile(file, miniature, userId);
    }

    public async Task<bool> DeleteTemporaryFileAsync(string fileName, string userId)
    {
        return await _attachmentsStorage.DeleteTemporaryFile(fileName, userId);
    }

    public async Task<FileDTO?> GetTemporaryThumbnailAsync(string fileName, string userId)
    {
        return  await _attachmentsStorage.GetTemporaryThumbnailAsync(fileName, userId);
    }

    public async Task<FileDTO?> GetTemporaryFileAsync(string fileName, string userId)
    {
        return await _attachmentsStorage.GetTemporaryFileAsync(fileName, userId);
    }

    public Task<IEnumerable<AttachmentDto>?> GetListOfTemporaryFile(string userId)
    {
        return _attachmentsStorage.GetListOfTemporaryFile(userId);
    }

    public async Task<FileDTO?> GetPermanentThumbnailAsync(string fileName, string taskId, string userId)
    {
        var task = await _taskRepository.FindAsync(taskId);
        if (task == null) return null;
        var user = await _usersApi.GetById(userId);
        if (user == null) return null;
        if (task.GroupName != user.Group) return null;
        return await _attachmentsStorage.GetPermanentThumbnail(fileName, taskId);
    }

    public async Task<FileDTO?> GetPermanentFileAsync(string fileName, string taskId, string userId)
    {
        var task = await _taskRepository.FindAsync(taskId);
        if (task == null) return null;
        var user = await _usersApi.GetById(userId);
        if (user == null) return null;
        if (task.GroupName != user.Group) return null;
        return await _attachmentsStorage.GetPermanentFile(fileName, taskId);
    }

    public async Task<bool> DeleteTaskAsync(string taskId, string userId)
    {
        var task = await _taskRepository.FindAsync(taskId);
        if (task == null) return false;
        
        if(task.UserID != userId) return false;

        bool success = true;
        foreach (var attachment in task.Attachments)
        {
            if(!(await _attachmentsStorage.DeletePermanentFile(attachment.FileName, taskId))) success = false;
        }

        if(success) success = await _taskRepository.DeleteAsync(taskId);
        
        return success;
    }
}