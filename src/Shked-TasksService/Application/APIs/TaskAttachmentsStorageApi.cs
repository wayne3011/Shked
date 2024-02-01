using System.Net;
using ShkedTasksService.Application.Infrastructure;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ShkedTasksService.Application.APIs.Options;
using ShkedTasksService.Application.DTO;
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
        _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        var temporaryFilesUri =
            new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath + _options.Value.TempFolder);
        MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
        
        var fileStreamContent = new StreamContent(file.OpenReadStream());
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        multipartFormDataContent.Add(fileStreamContent, _options.Value.FileKey, file.FileName);     
         
        var secondStreamContent = new StreamContent(thumbnail.OpenReadStream());
        secondStreamContent.Headers.ContentType = new MediaTypeHeaderValue(thumbnail.ContentType);
        multipartFormDataContent.Add(secondStreamContent, _options.Value.ThumbnailKey, file.FileName);

        var response = await _httpClient.PostAsync(temporaryFilesUri, multipartFormDataContent);
        if (response.StatusCode != HttpStatusCode.OK) return false;
        return true;
    }

    public async Task<bool> DeleteTemporaryFile(string fileName, string userId)
    {
        _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        var deleteTemporaryFileUri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath + _options.Value.TempFolder + fileName);
        var response = await _httpClient.DeleteAsync(deleteTemporaryFileUri);
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<AttachmentDto>?> MoveFilesToPermanentAsync(string userId, string taskId)
    {
        _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        var uri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath 
                                                    + _options.Value.TempFolder 
                                                    + _options.Value.ToPermanentMethod
                                                    + $"?taskId={taskId}");
        var response = await _httpClient.DeleteAsync(uri);
        if (response.IsSuccessStatusCode)
        {
            var strContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<AttachmentDto>>(strContent);
        }
        return null;
    }

    public async Task<IEnumerable<AttachmentDto>?> GetListOfTemporaryFile(string userId)
    {
        _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        var uri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath 
                                                    + _options.Value.TempFolder);
        var response = await _httpClient.GetAsync(uri);
        if (response.IsSuccessStatusCode)
        {
            var strContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<AttachmentDto>>(strContent);
        }

        return null;
    }

    public async Task<FileDTO?> GetTemporaryThumbnailAsync(string filename, string userId)
    {
        _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        var uri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath + _options.Value.TempFolder + _options.Value.ThumbnailsFolder + filename);
        var response = await _httpClient.GetAsync(uri);
        
        return response.IsSuccessStatusCode ? await ReadFileDto(response) : null;
    }
    
    public async Task<FileDTO?> GetTemporaryFileAsync(string filename, string userId)
    {
        _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        var uri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath + _options.Value.TempFolder + filename);
        var response = await _httpClient.GetAsync(uri);
        
        return response.IsSuccessStatusCode ? await ReadFileDto(response) : null;
    }

    public async Task<FileDTO?> GetPermanentThumbnail(string filename, string taskId)
    {
        var uri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath + taskId + '/' + _options.Value.ThumbnailsFolder + filename);
        var response = await _httpClient.GetAsync(uri);
        
        return response.IsSuccessStatusCode ? await ReadFileDto(response) : null;
    }

    public async Task<FileDTO?> GetPermanentFile(string filename, string taskId)
    {
        var uri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath + taskId + '/' + filename);
        var response = await _httpClient.GetAsync(uri);
        
        return response.IsSuccessStatusCode ? await ReadFileDto(response) : null;
    }

    public async Task<bool> DeletePermanentFile(string filename, string taskId)
    {
        var uri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath + taskId + '/' + filename);
        var response = await _httpClient.DeleteAsync(uri);
        return response.IsSuccessStatusCode;
    }

    private static async Task<FileDTO?> ReadFileDto(HttpResponseMessage response)
    {
        var fileStream = await response.Content.ReadAsStreamAsync();
        return new FileDTO()
        {
            FileStream = fileStream,
            ContentType = response.Content.Headers.ContentType.MediaType,
            LastModified = response.Content.Headers.LastModified.Value.Date,
            SizeKb = fileStream.Length,
            FileName = response.Content.Headers.ContentDisposition.FileName
        };
    }
    
}