using Microsoft.AspNetCore.Mvc;
using ShkedUsersService.Application.DTO;
using ShkedUsersService.Application.Infrastructure;

namespace ShkedUsersService.UI;
[Route("API/[controller]/")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }
    [Route("{id}")]
    [HttpGet]
    public async Task<IActionResult> GetById(string id)
    {
        var UserDto = await _usersService.GetById(id);
        return UserDto != null ? Ok(UserDto) : NotFound();
    }
}