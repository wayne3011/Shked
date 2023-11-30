
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
    [HttpPost]
    [Route("TEMP/")]
    public async Task<ActionResult<CreationResult>> CreateTemporaryFile(IFormFile file, IFormFile miniature)
    {
        if(!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)) return Unauthorized();
        var result = await _taskAttachmentsService.CreateTemporaryFileAsync(miniature, file, userIdHeader);
        if (result == null) return StatusCode(500);
        return result;

    }
}