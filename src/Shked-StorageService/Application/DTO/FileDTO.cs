using System.Text.Json.Serialization;

namespace ShkedStorageService.Application.DTO;

public class FileDTO
{
    [JsonPropertyName("fileName")]
    public string FileName { get; set; }
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; }
    [JsonPropertyName("sizeKb")]
    public long SizeKb { get; set; }
    [JsonPropertyName("extension")]
    public string Extension { get; set; }
    [JsonPropertyName("lastModified")]
    public DateTimeOffset LastModified { get; set; }
    [JsonPropertyName("fileStream")]
    public Stream FileStream { get; set; }
}