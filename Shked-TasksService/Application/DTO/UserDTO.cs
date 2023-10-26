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
// public class UserDTO
// {
//     public string Id { get; set; }
//     public string FullName { get; set; }
//     public string Email { get; set; }
//     public string Group { get; set; }
//     public IEnumerable<FriendGroup> FriendGroups { get; set; }
// }
//
// public class FriendGroup
// {
//     public string Title { get; set; }
//     public string Group { get; set; }
// }