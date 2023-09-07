using Moq;
using AutoMapper;
using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.DAL.Entities;
using SkedAuthorization.DAL.Infrastructure;

namespace Sked_Authorization.Tests.Base;

public class AuthServiceTestBase
{
    private List<User> _users = new()
    {
        new User
        {
            FullName = "Maks Ermilov",
            Email = "mermilov@gmail.com",
            PassHash = "$2a$12$lqreize/8WrO4bcJoZaEHOjQlhHVC3GqmGKe5hzOR5h/VQLhNJmc2",
            Group = "М3О-123Бк-23"
        },
        new User
        {
            FullName = "Ivan Petrov",
            Email = "ipetrov@ya.ru",
            PassHash = "$2a$12$SN9X3mF8siPsDc/GszWNCOXvRAZldeD84syHsPD6TBpyQLJJwXLzu",
            Group = "М3О-221Б-22"
        },
        new User
        {
            FullName = "Valentin Izmaylov",
            Email = "vitka@mail.ru",
            PassHash = "$2a$12$mcbPsLQm0pc1qARP2n7sY.41Wr/zgKmsWF/NMBsMxu3sXcaFS1zeK",
            Group = "М3О-324C-21"
        },
        new User
        {
            FullName = "Charles Bukowski",
            Email = "americanmen@yahoo.net",
            PassHash = "$2a$12$60sKhmcyropHBO5CNEXb9ef08S4qxQtJYKRjqZ8sIxmnwTMkHZSZS",
            Group = "М3О-426C-20"
        },
    };
    public IUserRepository GetUserRepo()
    {
        var mock = new Mock<IUserRepository>();
        mock.Setup(obj => obj.GetByEmail(It.IsAny<string>()))
            .Returns<string>(obj => Task.FromResult(_users.FirstOrDefault(x => x.Email == obj)));
        mock.Setup(obj => obj.GetById(It.IsAny<string>()))
            .Returns<string>(obj => Task.FromResult(_users.FirstOrDefault(x => x.Id == obj)));
        mock.Setup(obj => obj.Update(It.IsAny<User>())).Returns(() => Task.FromResult(true));
        mock.Setup(obj => obj.Delete(It.IsAny<string>())).Returns(() => Task.FromResult(true));
        return mock.Object;
    }

    public IMapper GetMapper()
    {
        var mock = new Mock<IMapper>();
        mock.Setup(obj => obj.Map<SignUpDTO, User>(It.IsAny<SignUpDTO>()))
            .Returns<SignUpDTO>((obj) => new User()
                { Email = obj.Email, PassHash = obj.Password, Group = obj.Group, FullName = obj.FullName });
        return mock.Object;
    } 
}