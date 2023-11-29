namespace ShkedTasksService.DAL.Entities;

public class Attachment
{
    public string Path { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string? Extension { get; set; }
    public long SizeKb { get; set; }
    public string Thumbnail { get; set; } = null!;
}