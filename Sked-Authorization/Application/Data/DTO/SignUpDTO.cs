using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkedAuthorization.Application.Data.DTO;

public class SignUpDTO
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("password")]
    public string Password { get; set; }
    [JsonPropertyName("group")]
    public string Group { get; set; }
}