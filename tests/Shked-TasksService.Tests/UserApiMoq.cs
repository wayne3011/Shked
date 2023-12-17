using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using ShkedTasksService.Application.Infrastructure;
using ShkedUsersService.Application.DTO;

namespace Shked_TasksService.Tests;

public class UserApiMoq
{
    public static IUsersApi Create()
    {
        var mock = new Mock<IUsersApi>();
        mock.Setup(x => x.GetById(It.IsAny<string>())).Returns<string>(userId => Task.FromResult(_users.Find(x => x.Id == userId)));
        return mock.Object;
    }

    private static List<UserDTO> _users = new List<UserDTO>()
    {
        new()
        {
            Id = "2bd04efb-cb61-4daf-a4ed-4022388da6a6",
            FullName = "Ермилов Максим",
            Email = "maks.ermilov@mai.education",
            Group = "М3О-325Бк-21",
            FriendGroups = new List<FriendGroup>()
        },
        new()
        {
            Id = "eb685107-5def-4d57-8c1d-f8c68be61e48\n",
            FullName = "Аня Великанова",
            Email = "ann.velik@mai.education",
            Group = "М3О-310Б-21",
            FriendGroups = new List<FriendGroup>()
        }
    };
}