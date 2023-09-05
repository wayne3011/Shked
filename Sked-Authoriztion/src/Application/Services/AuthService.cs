using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SkedAuthoriztion.Application.DTO;
using SkedAuthoriztion.Application.Infrastructure;
using SkedAuthoriztion.Application.Services.Options;
using SkedAuthoriztion.DAL.Entities;
using SkedAuthoriztion.DAL.Infrastructure;

namespace SkedAuthoriztion.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;
    private readonly IOptions<AuthOptions> _options;
    public AuthService(IUserRepository users, IMapper mapper, IOptions<AuthOptions> options)
    {
        _users = users;
        _mapper = mapper;
        _options = options;
    }
    
    public async Task<AuthDTO> SignUpAsync(SignUpDTO signUpDto)
    {
        var newUser = _mapper.Map<SignUpDTO, User>(signUpDto);
        newUser.Id = Guid.NewGuid().ToString();
        await _users.Create(newUser);
        return IssueToken(newUser.Id);
    }

    public async Task<AuthDTO?> SignInAsync(string email, string passHash)
    {
        var user = await _users.GetByEmail(email);
        if (user == null) return null;
        if (user.PassHash != passHash) return null; 
        return IssueToken(user.Id);
    }

    public async Task<AuthDTO> RefreshTokenAsync(string refreshToken)
    {
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Value.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Value.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Secret))
        };
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var claims = jwtSecurityTokenHandler.ValidateToken(refreshToken,tokenValidationParameters,out var outputToken);
        var userId = claims.Claims.First(x => x.Type == ClaimTypes.Name).Value;
        var user = await _users.GetById(userId);
        if (user == null) return null;
        if (!user.Devices.Contains(refreshToken)) return null;     
        return IssueToken(userId);
    }

    private AuthDTO IssueToken(string id)
    {
        var claims = new List<Claim>() { new Claim(ClaimTypes.Name, id) };

        var accessToken = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            claims: claims,
            signingCredentials: new SigningCredentials(_options.Value.SymmetricSecurityKey,
                SecurityAlgorithms.HmacSha256),
            expires: DateTime.UtcNow.AddMinutes(_options.Value.AccessTokenLifetimeMinutes)
            );
        var refreshToken = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            claims: claims,
            signingCredentials: new SigningCredentials(_options.Value.SymmetricSecurityKey,
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
}