using AutoMapper;
using Shked.UserDAL.Entities;
using Shked.UserDAL.Infrastructure;
using ShkedUsersService.Application.DTO;
using ShkedUsersService.Application.Infrastructure;

namespace ShkedUsersService.Application.Services;

public class UsersService : IUsersService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<UserDTO?> GetById(string id)
    {
        var user = await _userRepository.GetById(id);
        var userDto = _mapper.Map<User, UserDTO>(user);
        return userDto;
    }
}