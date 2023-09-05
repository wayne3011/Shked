using AutoMapper;
using SkedAuthorization.Application.DTO;
using SkedAuthorization.DAL.Entities;

namespace SkedAuthorization.Application.Mapper;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<SignUpDTO, User>();
    }
}