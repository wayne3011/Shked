using System.Text.Json.Serialization;

namespace SkedGroupsService.DAL.Models;

public class Lesson
{
    [JsonPropertyName("ordinal")]
    public int Ordinal { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("lecturer")]
    public string? Lecturer { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("location")]
    public string Location { get; set; }
}