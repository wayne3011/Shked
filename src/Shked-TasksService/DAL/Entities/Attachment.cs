namespace ShkedTasksService.DAL.Entities;

public class Attachment
{
    public string FileName { get; set; } = null!;
    public string? Extension { get; set; }
    public long SizeKb { get; set; }
}