namespace SkedGroupsService.Application.Models;

public class ParsingProgress
{
    public string Status { get; set; } 
    public Schedule? Schedule { get; set; }
}
public static class ParseStatus
{
    public static readonly string Starting = "ParsingStarted";
    public static readonly string Ended = "ParsingEnded";
    public static readonly string InternalError = "InternalError";
    public static readonly string Success = "Success";
} 