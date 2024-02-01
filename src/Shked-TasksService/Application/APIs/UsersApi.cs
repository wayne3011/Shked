using System.Text.Json;
using Microsoft.Extensions.Options;
using ShkedTasksService.Application.APIs.Options;
using ShkedTasksService.Application.Infrastructure;
using ShkedUsersService.Application.DTO;

namespace ShkedTasksService.Application.APIs;

public class UsersApi : IUsersApi
{
    private readonly IOptions<UsersApiOptions> _options;
    private readonly HttpClient _httpClient = new HttpClient();
    public UsersApi(IOptions<UsersApiOptions> options)
    {
        _options = options;
    }

    public async Task<UserDTO?> GetById(string id)
    {
        Uri uri = new Uri(_options.Value.ServiceUrl + _options.Value.CollectionName + $"/{id}");
        var response = await _httpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserDTO>(content);
    }
}