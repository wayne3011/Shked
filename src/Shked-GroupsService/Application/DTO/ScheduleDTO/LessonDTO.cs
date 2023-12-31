﻿using System.Text.Json.Serialization;

namespace ShkedGroupsService.Application.DTO.ScheduleDTO;

public class LessonDTO
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