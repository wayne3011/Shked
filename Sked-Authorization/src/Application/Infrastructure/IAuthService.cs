using SkedAuthorization.Application.DTO;

namespace SkedAuthorization.Application.Infrastructure;

public interface IAuthService
{
    Task<AuthDTO> SignUpAsync(SignUpDTO signUpDto);
    Task<AuthDTO?> SignInAsync(string email, string passHash);
    Task<AuthDTO?> RefreshTokenAsync(string refreshToken);
}