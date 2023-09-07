using System.Text.Json.Serialization;

namespace SkedAuthorization.Application.Data.DTO;

public class AuthDTO
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
    [JsonPropertyName("uuid")] 
    public string Id { get; set; }
    
}