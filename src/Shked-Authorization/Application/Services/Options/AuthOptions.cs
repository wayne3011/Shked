using System.Text;
using Microsoft.IdentityModel.Tokens;
using ShkedAuthorization.Application.Infrastructure;

namespace ShkedAuthorization.Application.Services.Options;
/// <summary>
/// Поставщик настроек авторизации для <see cref="IAuthService"/>
/// </summary>
public class AuthOptions
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecretAccess { get; set; } = null!;
    public string SecretRefresh { get; set; } = null!;
    /// <summary>
    /// Время жизни Access-токена в минутах
    /// </summary>
    public int AccessTokenLifetimeMinutes { get; set; }
    /// <summary>
    /// Время жизни Refresh-токена в днях
    /// </summary>
    public int RefreshTokenLifetimeDays { get; set; }
    /// <summary>
    /// Симметричный ключ для подписи Access-токенов
    /// </summary>
    public SymmetricSecurityKey SymmetricSecurityKeyAccess => new(Encoding.UTF8.GetBytes(SecretAccess));
    /// <summary>
    /// Симметричный ключ для подписи Refresh-токенов
    /// </summary>
    public SymmetricSecurityKey SymmetricSecurityKeyRefresh => new(Encoding.UTF8.GetBytes(SecretRefresh));
}