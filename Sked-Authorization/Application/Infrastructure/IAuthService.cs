using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.Application.Data.Responses;

namespace SkedAuthorization.Application.Infrastructure;

public interface IAuthService
{
    Task<AuthResult<AuthDTO>> SignUpAsync(SignUpDTO signUpDto);
    Task<AuthResult<AuthDTO>> SignInAsync(SignInDTO signInDto);
    Task<AuthResult<AuthDTO>> RefreshTokenAsync(string refreshToken);
    Task<AuthResult> Logout(string refreshToken);
    Task<AuthResult> LogoutFromAll(string id);
}