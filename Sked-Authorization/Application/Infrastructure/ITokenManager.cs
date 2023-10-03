using SkedAuthorization.Application.Data.DTO;

namespace SkedAuthorization.Application.Infrastructure;

public interface ITokenManager
{
    AuthDTO IssueToken(string id);
    string? GetUserId(string refreshToken);
}