using System.Text.Json.Serialization;

namespace ShkedStorageService.Application.DTO;

public class CreationResult
{
    [JsonPropertyName("filePath")]
    public string FilePath { get; set; } = null!;
    [JsonPropertyName("thumbnailPath")]
    public string ThumbnailPath { get; set; } = null!;
}