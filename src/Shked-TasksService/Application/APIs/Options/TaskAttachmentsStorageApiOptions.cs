namespace ShkedTasksService.Application.APIs.Options;

public class TaskAttachmentsStorageApiOptions
{
    public string ServiceUrl { get; set; }
    public string CollectionPath { get; set; }
    public string TempFolder { get; set; }
    public string ThumbnailsFolder { get; set; }
    public string FileKey { get; set; }
    public string ThumbnailKey { get; set; }
    public string ToPermanentMethod { get; set; }
}