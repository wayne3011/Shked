using System.Text.Json.Serialization;

namespace ShkedTasksService.Application.DTO;

public class AttachmentDto
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = null!;
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = null!;
    [JsonPropertyName("extension")]
    public string? Extension { get; set; }
    [JsonPropertyName("sizeKb")]
    public long SizeKb { get; set; }
    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; } = null!;
}