using ShkedStorageService.Application.DTO;

namespace ShkedStorageService.Application.Infrastructure;

public interface ITaskAttachmentsService
{
    Task<IEnumerable<string>> CreateAttachmentsAsync(IFormFileCollection fileStream, string taskId);
    Task<FileDTO> GetAttachmentAsync(string folder, string fileName);
}