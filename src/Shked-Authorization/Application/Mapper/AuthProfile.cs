using AutoMapper;
using Shked.UserDAL.Entities;
using ShkedAuthorization.Application.Data.DTO;


namespace ShkedAuthorization.Application.Mapper;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<SignUpDTO, User>();
    }
}