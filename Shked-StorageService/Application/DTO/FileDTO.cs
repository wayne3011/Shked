namespace ShkedStorageService.Application.DTO;

public class FileDTO
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public DateTime LastModified { get; set; }
    public Stream FileStream { get; set; }
}