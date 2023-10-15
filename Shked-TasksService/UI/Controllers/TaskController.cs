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
        taskDto.UserID = User.Identity.Name ?? "UNKNOWN";
        var paths = await _tasksService.CreateTaskAsync(taskDto, formFileCollection);
        return paths != null ? Ok(paths) : StatusCode(StatusCodes.Status500InternalServerError);
    }
}