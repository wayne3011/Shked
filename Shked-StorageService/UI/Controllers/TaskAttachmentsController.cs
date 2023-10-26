using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShkedStorageService.Application.Infrastructure;
using ShkedStorageService.Application.Services;

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
}