using System.Text.Json.Serialization;

namespace ShkedUsersService.Application.DTO;


public class UserDTO
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("group")]
    public string Group { get; set; }
    [JsonPropertyName("friendGroups")]
    public IEnumerable<FriendGroup> FriendGroups { get; set; }
}

public class FriendGroup
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("group")]
    public string Group { get; set; }
}