using System.Net;
using Microsoft.Extensions.Options;
using SkedGroupsService.Application.Infrastructure;
using SkedGroupsService.Application.Middlewares.Options;

namespace SkedGroupsService.Application.Middlewares;

public class ApiAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IApiKeyValidationService _validationService;
    private readonly IOptions<AuthOptions> _options;

    public ApiAuthMiddleware(RequestDelegate next, IApiKeyValidationService validationService, IOptions<AuthOptions> options)
    {
        _next = next;
        _validationService = validationService;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        string? userApiKey = httpContext.Request.Headers[_options.Value.ApiKeyHeaderName];
        if (string.IsNullOrEmpty(userApiKey))
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }

        if (!_validationService.IsValid(userApiKey))
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        await _next(httpContext);
    }
}