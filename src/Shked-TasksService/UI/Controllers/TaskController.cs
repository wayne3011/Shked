using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShkedTasksService.Application.DTO;
using ShkedTasksService.Application.Infrastructure;
using ShkedTasksService.UI.ModelBinders;

namespace ShkedTasksService.UI.Controllers;

[Route("API/[controller]/[action]")]
public class TaskController : ControllerBase
{
    private readonly ITasksService _tasksService;
    public TaskController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([ModelBinder(BinderType = typeof(JsonModelBinder))] TaskDTO taskDto, [FromForm] IFormFileCollection formFileCollection)
    {
        var userId = User.Identity.Name;
        if (userId == null) return Unauthorized();
        taskDto.UserID = userId;
        var paths = await _tasksService.CreateTaskAsync(taskDto, formFileCollection);
        return paths != null ? Ok(paths) : StatusCode(StatusCodes.Status500InternalServerError);
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetTasks()
    {
        var userId = User.Identity?.Name;
        if (userId == null) return Unauthorized();
        return Ok(await _tasksService.GetTasksAsync(userId));
    }
}