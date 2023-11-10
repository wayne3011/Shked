namespace SkedGroupsService.Application.Models;

public record GroupNameValidationResult(string? formattedGroupName, bool exists);