using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.Application.Infrastructure;

public interface ITaskAttachmentsStorageApi
{
    Task<bool> UploadTemporaryFile(IFormFile file, IFormFile thumbnail, string userId);
    Task<bool> DeleteTemporaryFile(string fileName, string userId);
}