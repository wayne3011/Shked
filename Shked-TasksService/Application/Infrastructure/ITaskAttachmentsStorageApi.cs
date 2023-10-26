namespace ShkedTasksService.Application.Infrastructure;

public interface ITaskAttachmentsStorageApi
{
    Task<IEnumerable<string>> CreateAttachments(IFormFileCollection files, string taskId);
}