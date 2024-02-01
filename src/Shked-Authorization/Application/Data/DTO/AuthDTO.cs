using System.Text.Json.Serialization;

namespace ShkedAuthorization.Application.Data.DTO;

public class AuthDTO : IEquatable<AuthDTO>
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
    [JsonPropertyName("uuid")] 
    public string Id { get; set; }

    public bool Equals(AuthDTO? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return AccessToken == other.AccessToken && RefreshToken == other.RefreshToken && Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AuthDTO)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(AccessToken, RefreshToken, Id);
    }
}