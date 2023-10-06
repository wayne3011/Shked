namespace SkedGroupsService.Application.Infrastructure;

public interface IApiKeyValidationService
{
    public bool IsValid(string apiKey);
}