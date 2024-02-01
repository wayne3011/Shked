using System.Text.Json.Serialization;

namespace ShkedTasksService.Application.DTO;

public class AttachmentDto
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = null!;
    [JsonPropertyName("extension")]
    public string? Extension { get; set; }
    [JsonPropertyName("sizeKb")]
    public long SizeKb { get; set; }
}