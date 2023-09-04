using AutoMapper;
using SkedAuthoriztion.Application.DTO;
using SkedAuthoriztion.DAL.Entities;

namespace SkedAuthoriztion.Application.Mapper;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<SignUpDTO, User>();
    }
}