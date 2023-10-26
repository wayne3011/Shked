using ShkedAuthorization.Application.Data.DTO;

namespace Sked_Authorization.Tests.Test.Data;

public class EmptyPropSignUpData : TheoryData<SignUpDTO>
{
    public EmptyPropSignUpData()
    {
        Add(new SignUpDTO()
        {
            Email = "",
            FullName = "Ivan Ivanov",
            Password = "$2a$16$ZGBbp6Ed4YH9Z2h8ELiR0.HeoI.ExdIANlsr0S1erMhd2.kVGWz06",
            Group = "М3О-100Бк-23"
        });
        Add(new SignUpDTO()
        {
            Email = "test@mail.ru",
            FullName = "",
            Password = "$2a$16$ZGBbp6Ed4YH9Z2h8ELiR0.HeoI.ExdIANlsr0S1erMhd2.kVGWz06",
            Group = "М3О-100Бк-23"
        });
        Add(new SignUpDTO()
        {
            Email = "test@mail.ru",
            FullName = "Ivan Ivanov",
            Password = "",
            Group = "М3О-100Бк-23"
        });
        Add(new SignUpDTO()
        {
            Email = "test@mail.ru",
            FullName = "Ivan Ivanov",
            Password = "$2a$16$ZGBbp6Ed4YH9Z2h8ELiR0.HeoI.ExdIANlsr0S1erMhd2.kVGWz06",
            Group = ""
        });
    }    
}