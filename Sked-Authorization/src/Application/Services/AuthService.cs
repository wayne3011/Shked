using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.Application.Data.Responses;
using SkedAuthorization.Application.Infrastructure;
using SkedAuthorization.Application.Services.Options;
using SkedAuthorization.DAL.Entities;
using SkedAuthorization.DAL.Infrastructure;
using SkedAuthorization.Application.Extensions;

namespace SkedAuthorization.Application.Services;

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
    
    public async Task<AuthResult<AuthDTO>> SignUpAsync(SignUpDTO signUpDto)
    {
        if(await _users.GetByEmail(signUpDto.Email) != null) return new AuthResult<AuthDTO>(null,AuthResultCode.EmailOccupied);
        var newUser = _mapper.Map<SignUpDTO, User>(signUpDto);
        newUser.Id = Guid.NewGuid().ToString();
        var authDto = IssueToken(newUser.Id);
        newUser.Devices = new List<string>() { authDto.RefreshToken };
        await _users.Create(newUser);
        return new AuthResult<AuthDTO>(authDto, AuthResultCode.Ok);
    }

    public async Task<AuthResult<AuthDTO>> SignInAsync(string email, string passHash)
    {
        var user = await _users.GetByEmail(email);
        if (user == null) return new AuthResult<AuthDTO>(null,AuthResultCode.InvalidEmail);
        if (user.PassHash != passHash) return new AuthResult<AuthDTO>(null,AuthResultCode.InvalidPass);
        var authDto = IssueToken(user.Id);
        (user.Devices as List<string>).Add(authDto.RefreshToken);
        await _users.Update(user);
        return new AuthResult<AuthDTO>(authDto,AuthResultCode.Ok);
    }

    public async Task<AuthResult<AuthDTO>> RefreshTokenAsync(string refreshToken)
    {

        var userId = GetUserId(refreshToken); 
        if(userId == null) return new AuthResult<AuthDTO>(null, AuthResultCode.InvalidRefreshToken);
        var user = await _users.GetById(userId);
        if (user == null) return new AuthResult<AuthDTO>(null,AuthResultCode.InvalidUserId);
        if (!user.Devices.Contains(refreshToken)) return new AuthResult<AuthDTO>(null,AuthResultCode.InvalidRefreshToken);
        var authDto = IssueToken(userId);
        (user.Devices as List<string>).ReplaceFirst(refreshToken, authDto.RefreshToken);
        await _users.Update(user);
        return new AuthResult<AuthDTO>(authDto,AuthResultCode.Ok);
    }

    public async Task<AuthResult> Logout(string refreshToken)
    {
        var userId = GetUserId(refreshToken);
        if (userId == null) return new AuthResult(AuthResultCode.InvalidRefreshToken);
        var user = await _users.GetById(userId);
        if (user == null) return new AuthResult(AuthResultCode.InvalidUserId);
        if((user.Devices as List<string>).Remove(refreshToken) == false) return new AuthResult(AuthResultCode.InvalidRefreshToken);
        await _users.Update(user);
        return new AuthResult(AuthResultCode.Ok);
    }

    public async Task<AuthResult> LogoutFromAll(string id)
    {
        var user = await _users.GetById(id);
        if (user == null) return new AuthResult(AuthResultCode.InvalidUserId);
        user.Devices = null;
        await _users.Update(user);
        return new AuthResult(AuthResultCode.Ok);
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

    private string? GetUserId(string refreshToken)
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