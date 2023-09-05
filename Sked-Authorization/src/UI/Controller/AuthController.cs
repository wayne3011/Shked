using SkedAuthorization.Application.DTO;
using SkedAuthorization.Application.Infrastructure;

namespace SkedAuthorization.UI.Controller;

using Microsoft.AspNetCore.Mvc;
[Route("API/[Controller]/[Action]")]
public class AuthController : Controller
{
    private readonly IAuthService _service;
    public AuthController(IAuthService service)
    {
        _service = service;
    }
    [HttpPost]
    public async Task<ActionResult<AuthDTO>> SignUp(SignUpDTO signUpDto)
    {
        return Ok(await _service.SignUpAsync(signUpDto));
    }
    [HttpGet]
    public async Task<ActionResult<AuthDTO>> SignIn([FromQuery]string email,[FromQuery] string passHash)
    {
        var authDto = await _service.SignInAsync(email, passHash);
        if (authDto == null) return Unauthorized();
        return Ok(authDto);
    }
    [HttpGet]
    public async Task<ActionResult<AuthDTO>> Refresh ([FromQuery]string refreshToken)
    {
        var authDto = await _service.RefreshTokenAsync(refreshToken);
        if (authDto == null) return null;
        return Ok(authDto);
    }
}