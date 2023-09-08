using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SkedAuthorization.Application.Services.Options;

public class AuthOptions
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecretAccess { get; set; } = null!;
    public string SecretRefresh { get; set; } = null!;
    public int AccessTokenLifetimeMinutes { get; set; }
    public int RefreshTokenLifetimeDays { get; set; }
    public SymmetricSecurityKey SymmetricSecurityKeyAccess => new(Encoding.UTF8.GetBytes(SecretAccess));
    public SymmetricSecurityKey SymmetricSecurityKeyRefresh => new(Encoding.UTF8.GetBytes(SecretRefresh));
}