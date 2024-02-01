using ShkedUsersService.Application.DTO;

namespace ShkedTasksService.Application.Infrastructure;

public interface IUsersApi
{
    Task<UserDTO?> GetById(string id);
}