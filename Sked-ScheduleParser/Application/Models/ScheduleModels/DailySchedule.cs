namespace SkedScheduleParser.Application.Models;

public class DailySchedule : IEquatable<DailySchedule>
{
    public List<string> Dates { get; set; } = new List<string>();
    public List<Lesson> Classes { get; set; } = new List<Lesson>();
    public string HashSum { get; set; } = "";

    public bool Equals(DailySchedule? other)
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
        return Equals((DailySchedule)obj);
    }

    public override int GetHashCode()
    {
        return HashSum.GetHashCode();
    }
}