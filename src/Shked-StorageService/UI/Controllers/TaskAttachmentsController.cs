
using Microsoft.AspNetCore.Mvc;

using ShkedStorageService.Application.DTO;
using ShkedStorageService.Application.Infrastructure;

namespace ShkedStorageService.UI.Controllers;
[Route("StorageAPI/[controller]/")]
public class TaskAttachmentsController : ControllerBase
{
    private readonly ITaskAttachmentsService _taskAttachmentsService;
    public TaskAttachmentsController(ITaskAttachmentsService taskAttachmentsService)
    {
        _taskAttachmentsService = taskAttachmentsService;       
    }
    [Route("Create/{taskId}")]
    [HttpPost]
    public async Task<IActionResult> TaskAttachmentsCreate([FromRoute]string taskId, [FromForm]IFormFileCollection attachments)
    {
        return Ok(await _taskAttachmentsService.CreateAttachmentsAsync(attachments, taskId));
    }
    [Route("{path}/{fileName}")]
    [HttpGet]
    public async Task<FileResult> GetTaskAttachments(string path, string fileName)
    {
        var fileDto = await _taskAttachmentsService.GetAttachmentAsync(path,fileName);
        return File(fileDto.FileStream, fileDto.ContentType,fileDto.FileName);
    }

    [HttpGet]
    [Route("TEMP/Thumbnails/{fileName}")]
    public async Task<IActionResult> GetTemporaryThumbnail(string fileName)
    {
        if(!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)) return Unauthorized();
        var fileDto = await _taskAttachmentsService.GetTemporaryThumbnail(userIdHeader, fileName);
    }
    [HttpPost]
    [Route("TEMP/")]
    public async Task<ActionResult<CreationResult>> CreateTemporaryFile(IFormFile file, IFormFile miniature)
    {
        if(!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)) return Unauthorized();
        var result = await _taskAttachmentsService.CreateTemporaryFileAsync(miniature, file, userIdHeader);
        if (result == null) return StatusCode(500);
        return result;

    }

    [HttpDelete]
    [Route("TEMP/{taskId}")]
    public async Task<ActionResult> MoveToPermanentFiles(string taskId)
    {   
        if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)) return Unauthorized();
        var result = await _taskAttachmentsService.MoveToPermanentFiles(userIdHeader, taskId);
        if (!result) return StatusCode(500);
        return Ok();
    }
}