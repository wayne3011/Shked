using AutoMapper;
using FluentValidation;
using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.Application.Data.Responses;
using SkedAuthorization.Application.Infrastructure;
using SkedAuthorization.DAL.Entities;
using SkedAuthorization.DAL.Infrastructure;
using SkedAuthorization.Application.Extensions;

namespace SkedAuthorization.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;
    private readonly ITokenManager _tokenManager;
    private readonly IValidator<SignUpDTO> _signUpValidator;
    private readonly IValidator<SignInDTO> _signInValidator;
    public AuthService(IUserRepository users, IMapper mapper, IValidator<SignUpDTO> signUpValidator, IValidator<SignInDTO> signInValidator, ITokenManager tokenManager)
    {
        _users = users;
        _mapper = mapper;
        _signUpValidator = signUpValidator;
        _signInValidator = signInValidator;
        _tokenManager = tokenManager;
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
        newUser.PassHash = BCrypt.Net.BCrypt.HashPassword(signUpDto.Password, 16);
        
        var authDto = _tokenManager.IssueToken(newUser.Id);
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
                new (){ ErrorCode = (int)AuthResultCode.InvalidCredentials, ErrorMessage = "Invalid credentials." }
            });
        }

        if (!BCrypt.Net.BCrypt.Verify(signInDto.Password, user.PassHash))
        {
            return new AuthResult<AuthDTO>(null, new List<ValidationError>
            {
                new(){ ErrorCode = (int)AuthResultCode.InvalidCredentials, ErrorMessage = "Invalid credentials." }
            });
        }
        
        var authDto = _tokenManager.IssueToken(user.Id);
        (user.Devices as List<string>).Add(authDto.RefreshToken);
        await _users.Update(user);
        
        return new AuthResult<AuthDTO>(authDto,null);
    }

    public async Task<AuthResult<AuthDTO>> RefreshTokenAsync(string refreshToken)
    {

        var userId = _tokenManager.GetUserId(refreshToken); 
        if(userId == null) return new AuthResult<AuthDTO>(null, new List<ValidationError>
        {
            new ValidationError{ ErrorCode = (int)AuthResultCode.InvalidRefreshToken, ErrorMessage = "Invalid Refresh Token." }
        });
        var user = await _users.GetById(userId);
        if (user == null || !user.Devices.Contains(refreshToken)) return new AuthResult<AuthDTO>(null, new List<ValidationError>
        {
            new ValidationError{ ErrorCode = (int)AuthResultCode.InvalidRefreshToken, ErrorMessage = "Invalid Refresh Token." }
        });
        var authDto = _tokenManager.IssueToken(userId);
        (user.Devices as List<string>).ReplaceFirst(refreshToken, authDto.RefreshToken);
        await _users.Update(user);
        return new AuthResult<AuthDTO>(authDto,null);
    }

    public async Task<AuthResult> Logout(string refreshToken)
    {
        var userId = _tokenManager.GetUserId(refreshToken);
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
}