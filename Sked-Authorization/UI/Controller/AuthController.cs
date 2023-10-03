using Microsoft.AspNetCore.Authorization;
using SkedAuthorization.Application.Data.DTO;
using SkedAuthorization.Application.Data.Responses;
using SkedAuthorization.Application.Infrastructure;
using SkedAuthorization.Application.Services;

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
    public async Task<ActionResult<AuthDTO>> SignUp([FromBody]SignUpDTO signUpDto)
    {
        var result = await _service.SignUpAsync(signUpDto);
        if (result.ValidateErrors != null)
        {
            return BadRequest(result.ValidateErrors);
        }
        return Ok(result.Value);
    }
    [HttpGet]
    public async Task<ActionResult<AuthDTO>> SignIn([FromQuery] SignInDTO signInDto)
    {
        var result = await _service.SignInAsync(signInDto);
        if (result.ValidateErrors != null) return BadRequest(result.ValidateErrors);
        return Ok(result.Value);
    }
    [HttpGet]
    public async Task<ActionResult<AuthDTO>> Refresh ([FromQuery]string refreshToken)
    {
        var result = await _service.RefreshTokenAsync(refreshToken);
        if (result.ValidateErrors != null) return BadRequest(result.ValidateErrors);
        return Ok(result.Value);
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> Logout([FromQuery] string refreshToken)
    {
        var result = await _service.Logout(refreshToken);
        if (result.Code == AuthResultCode.InvalidUserId)
        {
            var error = new ErrorViewModel()
            {
                errorCode = (int)result.Code,
                errorMsg = "Invalid User ID."
            };
            return NotFound(error);
        }

        if (result.Code == AuthResultCode.InvalidRefreshToken)
        {
            var error = new ErrorViewModel()
            {
                errorCode = (int)result.Code,
                errorMsg = "Invalid Refresh Token."
            };
        }

        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> LogoutFromAll()
    {
        var result = await _service.LogoutFromAll(this.HttpContext.User.Identity.Name);
        if (result.Code == AuthResultCode.InvalidUserId)
        {
            var error = new ErrorViewModel()
            {
                errorCode = (int)result.Code,
                errorMsg = "Invalid User ID."
            };
            return NotFound(error);
        }

        return Ok();
    }
}