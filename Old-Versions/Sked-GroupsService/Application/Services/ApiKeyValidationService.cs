using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SkedGroupsService.Application.Infrastructure;
using SkedGroupsService.Application.Middlewares.Options;

namespace SkedGroupsService.Application.Kafka;

public class ApiKeyValidationService : IApiKeyValidationService
{
    private readonly IOptions<AuthOptions> _authOptions;

    public ApiKeyValidationService(IOptions<AuthOptions> authOptions)
    {
        _authOptions = authOptions;
    }
    public bool IsValid(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey)) return false;
        if (apiKey != _authOptions.Value.ApiSecretKey) return false;
        return true;
    }
}