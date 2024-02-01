using ShkedUsersService.Application.DTO;

namespace ShkedUsersService.Application.Infrastructure;

public interface IUsersService
{
    Task<UserDTO?> GetById(string id);
}