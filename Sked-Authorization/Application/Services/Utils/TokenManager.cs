using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.Application.Infrastructure;
using SkedAuthorization.Application.Services.Options;

namespace SkedAuthorization.Application.Services.Utils;

public class TokenManager : ITokenManager
{
    private readonly IOptions<AuthOptions> _options;

    public TokenManager(IOptions<AuthOptions> options)
    {
        _options = options;
    }
    public AuthDTO IssueToken(string id)
    {
        var claims = new List<Claim>() { new Claim(ClaimTypes.Name, id) };

        var accessToken = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            claims: claims,
            signingCredentials: new SigningCredentials(_options.Value.SymmetricSecurityKeyAccess,
                SecurityAlgorithms.HmacSha256),
            expires: DateTime.UtcNow.AddMinutes(_options.Value.AccessTokenLifetimeMinutes)
        );
        var refreshToken = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            claims: claims,
            signingCredentials: new SigningCredentials(_options.Value.SymmetricSecurityKeyRefresh,
                SecurityAlgorithms.HmacSha256),
            expires: DateTime.UtcNow.AddDays(_options.Value.RefreshTokenLifetimeDays)
        );
        var jwtHandler = new JwtSecurityTokenHandler();
        AuthDTO authDto = new()
        {
            Id = id,
            AccessToken = jwtHandler.WriteToken(accessToken),
            RefreshToken = jwtHandler.WriteToken(refreshToken)
        };
        return authDto;
    }

    public string? GetUserId(string refreshToken)
    {
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Value.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _options.Value.SymmetricSecurityKeyRefresh
        };
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal claims;
        try
        {
            claims = jwtSecurityTokenHandler.ValidateToken(refreshToken,tokenValidationParameters,out var outputToken);
        }
        catch (ArgumentException e)
        {
            return null;
        }
        return claims.Claims.First(x => x.Type == ClaimTypes.Name).Value;
    }
}