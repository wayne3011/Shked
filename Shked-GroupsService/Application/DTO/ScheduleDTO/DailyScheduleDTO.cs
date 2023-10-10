using System.Text.Json.Serialization;
using ShkedGroupsService.Application.DTO.ScheduleDTO.JsonConverters;

namespace ShkedGroupsService.Application.DTO.ScheduleDTO;

public class DailyScheduleDTO : IEquatable<DailyScheduleDTO>
{
    public List<DateOnly> Dates { get; set; } = new List<DateOnly>();
    public List<LessonDTO> Classes { get; set; } = new List<LessonDTO>();
    public string HashSum { get; set; } = "";

    public bool Equals(DailyScheduleDTO? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return HashSum == other.HashSum;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((DailyScheduleDTO)obj);
    }

    public override int GetHashCode()
    {
        return HashSum.GetHashCode();
    }
}