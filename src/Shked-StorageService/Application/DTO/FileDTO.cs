namespace ShkedStorageService.Application.DTO;

public class FileDTO
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public Stream FileStream { get; set; }
}