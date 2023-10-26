using ShkedAuthorization.Application.Data.DTO;

namespace Sked_Authorization.Tests.Test.Data;

public class SimplePassSignUpData : TheoryData<SignUpDTO>
{
    public SimplePassSignUpData() {
    Add(new SignUpDTO()
    {
        Email = "ivashka@mail.ru",
        FullName = "Ivan Ivanov",
        Password = "123456",
        Group = "М3О-100Бк-23"
    });
    Add(new SignUpDTO()
    {
        Email = "test@mail.ru",
        FullName = "Sergey Shipilov",
        Password = "Qwerty",
        Group = "М3О-100Бк-23"
    });
    Add(new SignUpDTO()
    {
        Email = "test@mail.ru",
        FullName = "Ivan Ivanov",
        Password = "qwerty1",
        Group = "М3О-100Бк-23"
    });
    Add(new SignUpDTO()
    {
        Email = "test@mail.ru",
        FullName = "Ivan Ivanov",
        Password = "qwerty",
        Group = "М3О-100Бк-23"
    });
}  
}