using AutoMapper;
using Shked.UserDAL.Entities;
using ShkedUsersService.Application.DTO;

namespace ShkedUsersService.Application.Mapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>();
    }
}