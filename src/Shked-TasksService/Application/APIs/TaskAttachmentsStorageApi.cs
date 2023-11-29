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
    public async Task<IEnumerable<Attachment>?> CreateAttachments(IFormFileCollection files, string taskId)
    {
        var form = new MultipartFormDataContent();
        List<Attachment> attachments = new List<Attachment>();
        foreach (var file in files)
        {
            var fileExtension = file.FileName.Split(new[] { '.' }).TakeLastIfNotOnly();
            var fileName = fileExtension is null ? file.FileName : file.FileName.Remove(file.FileName.LastIndexOf('.'));
            var attachment = new Attachment()
            {
                Extension = fileExtension,
                FileName = fileName,
                SizeKb = file.Length / 1024,
                Thumbnail = "none"
            };
            attachments.Add(attachment);
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            form.Add(streamContent, _options.Value.FilesKey, file.FileName);
        }

        Uri uri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath + "/Create" + $"/{taskId}");
        var response = await _httpClient.PostAsync(uri, form);
        var content = await response.Content.ReadAsStringAsync();
        var paths = JsonSerializer.Deserialize<List<string>>(content);
        if (paths != null)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                attachments[i].Path = paths[i];
            }
        }
        else return null;
        
        return attachments;
    }

}