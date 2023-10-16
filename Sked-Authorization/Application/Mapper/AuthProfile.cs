using AutoMapper;
using Shked.UserDAL.Entities;
using SkedAuthorization.Application.Data.DTO;


namespace SkedAuthorization.Application.Mapper;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<SignUpDTO, User>();
    }
}