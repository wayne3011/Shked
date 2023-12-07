using System.Text.Json.Serialization;

namespace ShkedTasksService.Application.DTO;

public class TaskDTO
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("userID")]
    public string UserID { get; set; }
    [JsonPropertyName("deadline")]
    public DateTime Deadline { get; set; }
    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; }
    [JsonPropertyName("lessonOrdinal")]
    public int LessonOrdinal { get; set; }
    [JsonPropertyName("text")]
    public string Text { get; set; }
    [JsonPropertyName("attachments")]
    public IEnumerable<AttachmentDto> Attachments { get; set; }
}