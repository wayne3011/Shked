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

    public async Task<bool> UploadTemporaryFile(IFormFile file, IFormFile miniature, string userId)
    {
        return await _attachmentsStorage.UploadTemporaryFile(file, miniature, userId);
    }

    public Task<bool> DeleteTemporaryFile(string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<AttachmentDto> GetTemporaryThumbnail(string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<AttachmentDto> GetTemporaryFile(string userId)
    {
        throw new NotImplementedException();
    }
}