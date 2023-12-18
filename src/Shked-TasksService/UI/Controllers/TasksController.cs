using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using ShkedTasksService.Application.DTO;
using ShkedTasksService.Application.Infrastructure;
using ShkedTasksService.UI.ModelBinders;

namespace ShkedTasksService.UI.Controllers;

[Route("API/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITasksService _tasksService;
    public TasksController(ITasksService tasksService)
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

    [HttpGet]
    [Authorize]
    [Route("TEMP/")]
    public async Task<ActionResult<AttachmentDto>> GetTemporaryFiles()
    {
        string userId = HttpContext.User.Identity.Name;
        if (userId == null) return Unauthorized();
        var result = await _tasksService.GetListOfTemporaryFile(userId);
        if (result == null) return StatusCode((int)HttpStatusCode.InternalServerError);
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    [Route("TEMP/{fileName}")]
    public async Task<ActionResult> GetTemporaryFile(string fileName)
    {
        string userId = HttpContext.User.Identity.Name;
        if (userId == null) return Unauthorized();
        var result = await _tasksService.GetTemporaryFileAsync(fileName, userId);
        if (result == null) return StatusCode((int)HttpStatusCode.InternalServerError);
        return File(result.FileStream, result.ContentType, result.LastModified, 
            new EntityTagHeaderValue(new StringSegment('"' + result.LastModified.ToString() + '"')));
    }
    [HttpGet]
    [Authorize]
    [Route("TEMP/Thumbnails/{fileName}")]
    public async Task<ActionResult> GetTemporaryThumbnail(string fileName)
    {
        string userId = HttpContext.User.Identity.Name;
        if (userId == null) return Unauthorized();
        var result = await _tasksService.GetTemporaryThumbnailAsync(fileName, userId);
        if (result == null) return StatusCode((int)HttpStatusCode.InternalServerError);
        return File(result.FileStream, result.ContentType, result.LastModified, 
            new EntityTagHeaderValue(new StringSegment('"' + result.LastModified.ToString() + '"')));
    }

    [HttpGet]
    [Authorize]
    [Route("{taskId}/{fileName}")]
    public async Task<ActionResult> GetPermanentFile(string fileName, string taskId)
    {
        string userId = HttpContext.User.Identity.Name;
        if (userId == null) return Unauthorized();
        var result = await _tasksService.GetPermanentFileAsync(fileName, taskId, userId);
        if (result == null) return StatusCode((int)HttpStatusCode.InternalServerError);
        return File(result.FileStream, result.ContentType, result.LastModified, 
            new EntityTagHeaderValue(new StringSegment('"' + result.LastModified.ToString() + '"')));
    }
    [HttpGet]
    [Authorize]
    [Route("{taskId}/Thumbnails/{fileName}")]
    public async Task<ActionResult> GetPermanentThumbnailFile(string fileName, string taskId)
    {
        string userId = HttpContext.User.Identity.Name;
        if (userId == null) return Unauthorized();
        var result = await _tasksService.GetPermanentThumbnailAsync(fileName, taskId, userId);
        if (result == null) return StatusCode((int)HttpStatusCode.InternalServerError);
        return File(result.FileStream, result.ContentType, result.LastModified, 
            new EntityTagHeaderValue(new StringSegment('"' + result.LastModified.ToString() + '"')));
    }

    [HttpDelete]
    [Authorize]
    [Route("{taskId}")]
    public async Task<ActionResult> DeleteTask(string taskId)
    {
        string userId = HttpContext.User.Identity.Name;
        if (userId == null) return Unauthorized();
        var result = await _tasksService.DeleteTaskAsync(taskId, userId);
        if(!result) return StatusCode((int)HttpStatusCode.InternalServerError);
        return NoContent();
    }
}