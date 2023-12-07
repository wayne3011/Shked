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

    public Task<AttachmentDto> GetTemporaryThumbnailAsync(string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<AttachmentDto> GetTemporaryFileAsync(string userId)
    {
        throw new NotImplementedException();
    }
}