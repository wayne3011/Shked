using System.Net;
using ShkedTasksService.Application.Infrastructure;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ShkedTasksService.Application.APIs.Options;
using ShkedTasksService.Application.Extensions;
using ShkedTasksService.DAL.Entities;

namespace ShkedTasksService.Application.APIs;

public class TaskAttachmentsStorageApi : ITaskAttachmentsStorageApi
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<TaskAttachmentsStorageApiOptions> _options;
    public TaskAttachmentsStorageApi(IOptions<TaskAttachmentsStorageApiOptions> options)
    {
        _options = options;
        _httpClient = new HttpClient();
    }
    public async Task<bool> UploadTemporaryFile(IFormFile file, IFormFile thumbnail, string userId)
    {
        var temporaryFilesUri =
            new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath + _options.Value.TempFolder);
        MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
        
        var fileStreamContent = new StreamContent(file.OpenReadStream());
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        multipartFormDataContent.Add(fileStreamContent, _options.Value.FileKey, file.FileName);     
        
        var secondStreamContent = new StreamContent(file.OpenReadStream());
        secondStreamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        multipartFormDataContent.Add(secondStreamContent, _options.Value.ThumbnailKey, file.FileName);
        
        multipartFormDataContent.Add(fileStreamContent);
        multipartFormDataContent.Add(secondStreamContent);

        var response = await _httpClient.PostAsync(temporaryFilesUri, multipartFormDataContent);
        if (response.StatusCode != HttpStatusCode.OK) return false;
        return true;
    }

    public Task<bool> DeleteTemporaryFile(string fileName, string userId)
    {
        var temporary;
    }
}