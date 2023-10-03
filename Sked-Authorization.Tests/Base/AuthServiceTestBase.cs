using Moq;
using AutoMapper;
using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.DAL.Entities;
using SkedAuthorization.DAL.Infrastructure;


namespace Sked_Authorization.Tests.Base;

public class AuthServiceTestBase
{
    protected List<User> _users = new()
    {
        //password: Qwerty1
        new User
        {
            Id = "e99de757-0c5e-46d9-9e7c-59578e22a5bb",
            FullName = "Maks Ermilov",
            Email = "mermilov@gmail.com",
            PassHash = "$2a$10$dLJ1f6k93x.cVLdNER0e2e7SvVgHxoQ9VBQX6nGmWU43KVxpBWciW",
            Group = "М3О-123Бк-23",
            Devices = new List<string>()
        },
        //password: ytrewqW1
        new User
        {
            Id = "36a25acb-2bcd-4973-9090-f5c8f9234d28",
            FullName = "Ivan Petrov",
            Email = "ipetrov@ya.ru",
            PassHash = "$2a$10$L2vrDaYTfwU1S5kLgyZCJ.7c7eRRew6kdQlVLct5EdgGvQcE95r4a",
            Group = "М3О-221Б-22",
            Devices = new List<string>()
        },
        //password: 12345Yg
        new User
        {
            Id = "84859d87-e5b6-44d5-bb72-574c962f4f9e",
            FullName = "Valentin Izmaylov",
            Email = "vitka@mail.ru",
            PassHash = "$2a$10$kl8FxItlirWABUYHbAlule2CFKItYuZX.4DYjRawRN8N4a04lHehu",
            Group = "М3О-324C-21",
            Devices = new List<string>()
            {
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9" +
                ".eyJpc3MiOiJJc3N1ZXIiLCJpYXQiOjE2OTQxOTYyMjgsImV4cCI6MTkxNTAzNDYyOCwiYXVkIjoiQXVkaWVuY2UiLCJzdWIiOiIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiODM2OTBmZjItNDVhYS00ODg2LTkxYjgtYjNlYmNkMGRkZmFlIn0" +
                ".1g9N8Uf-9U68-u-ivZTD9eo8-V8IDzmmEPuHGSsjcC8",
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9" +
                ".eyJpc3MiOiJJc3N1ZXIiLCJpYXQiOjE2OTQyMDI0MTMsImV4cCI6MTcyNTczODQxNCwiYXVkIjoiQXVkaWVuY2UiLCJzdWIiOiIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiODM2OTBmZjItNDVhYS00ODg2LTkxYjgtYjNlYmNkMGRkZmFlIn0" +
                ".d9FxE5cPsHHYhookKRGDXeYHed6rj4R0R3GDEN6qOeo"
            }
        },
        //password: tyuiPh1234
        new User
        {
            Id = "10532da3-b9ea-4283-8063-46a129d4f680",
            FullName = "Charles Bukowski",
            Email = "americanmen@yahoo.net",
            PassHash = "$2a$10$rouFofXU1pVT6AJ9cDdyWeF3fab0zhkpEMgBOSJjV.Z8kLRi9jLYO",
            Group = "М3О-426C-20",
            Devices = new List<string>()
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