using ShkedTasksService.Application.Infrastructure;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ShkedTasksService.Application.APIs.Options;

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
    public async Task<IEnumerable<string>> CreateAttachments(IFormFileCollection files, string taskId)
    {
        var form = new MultipartFormDataContent();
        foreach (var file in files)
        {
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            form.Add(streamContent, _options.Value.FilesKey, file.FileName);
        }

        Uri uri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionPath + "/Create" + $"/{taskId}");
        var response = await _httpClient.PostAsync(uri, form);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IEnumerable<string>>(content);
    }

}