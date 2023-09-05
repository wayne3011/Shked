using SkedAuthoriztion.Application.DTO;
using SkedAuthoriztion.Application.Infrastructure;

namespace SkedAuthoriztion.UI.Controller;

using Microsoft.AspNetCore.Mvc;
[Route("API/{Controller}/")]
public class AuthController : Controller
{
    private readonly IAuthService _service;
    public AuthController(IAuthService service)
    {
        _service = service;
    }

    public async Task<ActionResult<AuthDTO>> SignUp(SignUpDTO signUpDto)
    {
        return Ok(await _service.SignUpAsync(signUpDto));
    }

    public async Task<ActionResult<AuthDTO>> SignIn([FromQuery]string email,[FromQuery] string passHash)
    {
        var authDto = await _service.SignInAsync(email, passHash);
        if (authDto == null) return Unauthorized();
        return Ok(authDto);
    }
    public async Task<ActionResult<AuthDTO>> Refresh ([FromQuery]string refreshToken)
    {
        var authDto = await _service.RefreshTokenAsync(refreshToken);
        if (authDto == null) return null;
        return Ok(authDto);
    }
}