using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShkedTasksService.Application.DTO;
using ShkedTasksService.Application.Infrastructure;
using ShkedTasksService.UI.ModelBinders;

namespace ShkedTasksService.UI.Controllers;

[Route("API/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITasksService _tasksService;
    public TaskController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetTasks()
    {
        var userId = User.Identity?.Name;
        if (userId == null) return Unauthorized();
        return Ok(await _tasksService.GetTasksAsync(userId));
    }

    [HttpPost]
    [Authorize]
    [Route("TEMP/")]
    public async Task<IActionResult> UploadTemporaryFile(IFormFile file, IFormFile thumbnail)
    {
        string userId = HttpContext.User.Identity.Name;
        if (userId == null) return Unauthorized();
        var result = await _tasksService.UploadTemporaryFileAsync(file, thumbnail, userId);
        if (result) return Ok();
        return StatusCode((int)HttpStatusCode.InternalServerError);

    }

    [HttpDelete]
    [Authorize]
    [Route("TEMP/{fileName}")]
    public async Task<IActionResult> DeleteTemporaryFile(string fileName)
    {
        string userId = HttpContext.User.Identity.Name;
        if (userId == null) return Unauthorized();
        var result = await _tasksService.DeleteTemporaryFileAsync(fileName, userId);
        if (result) return Ok();
        return StatusCode((int)HttpStatusCode.InternalServerError);
    }

    [HttpPost]
    [Authorize]
    [Route("")]
    public async Task<ActionResult<TaskDTO>> CreateTask([FromBody]TaskDTO taskDto)
    {
        string userId = HttpContext.User.Identity.Name;
        if (userId == null) return Unauthorized();
        var result = await _tasksService.CreateTaskAsync(userId, taskDto);
        if (result == null) return StatusCode((int)HttpStatusCode.InternalServerError);
        return Ok(taskDto);
    }
}