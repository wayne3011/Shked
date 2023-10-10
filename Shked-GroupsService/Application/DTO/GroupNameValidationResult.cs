namespace ShkedGroupsService.Application.DTO;

public record GroupNameValidationResult(string? formattedGroupName, bool exists);