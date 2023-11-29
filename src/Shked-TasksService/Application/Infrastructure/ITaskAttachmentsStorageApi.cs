using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.Application.Infrastructure;

public interface ITaskAttachmentsStorageApi
{
    Task<IEnumerable<Attachment>?> CreateAttachments(IFormFileCollection files, string taskId);
}