using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FluentValidation;
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
    private readonly IValidator<SignUpDTO> _signUpValidator;
    private readonly IValidator<SignInDTO> _signInValidator;
    public AuthService(IUserRepository users, IMapper mapper, IOptions<AuthOptions> options, IValidator<SignUpDTO> signUpValidator, IValidator<SignInDTO> signInValidator)
    {
        _users = users;
        _mapper = mapper;
        _options = options;
        _signUpValidator = signUpValidator;
        _signInValidator = signInValidator;
    }
    
    public async Task<AuthResult<AuthDTO>> SignUpAsync(SignUpDTO signUpDto)
    {
        var validationResult = await _signUpValidator.ValidateAsync(signUpDto);
        if (!validationResult.IsValid)
        {
            return new AuthResult<AuthDTO>(null, validationResult.Errors.ToValidationErrorsList());
        }
        
        var newUser = _mapper.Map<SignUpDTO, User>(signUpDto);
        newUser.Id = Guid.NewGuid().ToString();
        
        var authDto = IssueToken(newUser.Id);
        newUser.Devices = new List<string> { authDto.RefreshToken };
        await _users.Create(newUser);
        
        return new AuthResult<AuthDTO>(authDto, null);
    }

    public async Task<AuthResult<AuthDTO>> SignInAsync(SignInDTO signInDto)
    {
        var validateResult = await _signInValidator.ValidateAsync(signInDto);
        if (!validateResult.IsValid)
        {
            return new AuthResult<AuthDTO>(null, validateResult.Errors.ToValidationErrorsList());
        }
        var user = await _users.GetByEmail(signInDto.Email);
        if (user == null)
        {
            return new AuthResult<AuthDTO>(null,new List<ValidationError>
            {
                new (){ ErrorCode = (int)AuthResultCode.InvalidEmail, ErrorMessage = "Invalid Email Address." }
            });
        }

        if (user.PassHash != signInDto.PassHash)
        {
            return new AuthResult<AuthDTO>(null, new List<ValidationError>
            {
                new(){ ErrorCode = (int)AuthResultCode.InvalidPass, ErrorMessage = "Invalid Pass." }
            });
        }
        
        var authDto = IssueToken(user.Id);
        (user.Devices as List<string>).Add(authDto.RefreshToken);
        await _users.Update(user);
        
        return new AuthResult<AuthDTO>(authDto,null);
    }

    public async Task<AuthResult<AuthDTO>> RefreshTokenAsync(string refreshToken)
    {

        var userId = GetUserId(refreshToken); 
        if(userId == null) return new AuthResult<AuthDTO>(null, new List<ValidationError>
        {
            new ValidationError{ ErrorCode = (int)AuthResultCode.InvalidRefreshToken, ErrorMessage = "Invalid Refresh Token." }
        });
        var user = await _users.GetById(userId);
        if (user == null || !user.Devices.Contains(refreshToken)) return new AuthResult<AuthDTO>(null, new List<ValidationError>
        {
            new ValidationError{ ErrorCode = (int)AuthResultCode.InvalidRefreshToken, ErrorMessage = "Invalid Refresh Token." }
        });
        var authDto = IssueToken(userId);
        (user.Devices as List<string>).ReplaceFirst(refreshToken, authDto.RefreshToken);
        await _users.Update(user);
        return new AuthResult<AuthDTO>(authDto,null);
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