
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
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
    [HttpGet]
    [Route("TEMP/Thumbnails/{fileName}")]
    public async Task<IActionResult> GetTemporaryThumbnail(string fileName)
    {
        if(!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)) return Unauthorized();
        var fileDto = await _taskAttachmentsService.GetTemporaryThumbnailAsync(userIdHeader, fileName);
        return File(fileDto.FileStream, fileDto.ContentType, fileDto.FileName, fileDto.LastModified, new EntityTagHeaderValue('"' + fileDto.LastModified.ToString() + '"'));
    }

    [HttpGet]
    [Route("TEMP/")]
    public async Task<ActionResult<IEnumerable<FileDTO>>> GetListOfTemporaryFile()
    {
        if(!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)) return Unauthorized();
        var fileDtoLists = await _taskAttachmentsService.GetListOfTemporaryFiles(userIdHeader);
        if (fileDtoLists == null) return StatusCode(500);
        return Ok(fileDtoLists);
    }
    [HttpGet]
    [Route("TEMP/{fileName}")]
    public async Task<IActionResult> GetTemporaryFile(string fileName)
    {
        if(!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)) return Unauthorized();
        var fileDto = await _taskAttachmentsService.GetTemporaryFileAsync(userIdHeader, fileName);
        return File(fileDto.FileStream, fileDto.ContentType, fileDto.FileName, fileDto.LastModified, new EntityTagHeaderValue('"' + fileDto.LastModified.ToString() + '"'));
    }
    
    [HttpPost]
    [Route("TEMP/")]
    public async Task<ActionResult<CreationResult>> CreateTemporaryFile([FromForm]IFormFile file, [FromForm]IFormFile thumbnail)
        {
        if(!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)) return Unauthorized();
        var result = await _taskAttachmentsService.CreateTemporaryFileAsync(thumbnail, file, userIdHeader);
        if (result == null) return StatusCode(500);
        return result;

    }

    [HttpDelete]
    [Route("TEMP/ToPermanent")]
    public async Task<ActionResult<List<FileDTO>>> MoveToPermanentFiles([FromQuery] string taskId)
    {   
        if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)) return Unauthorized();
        var result = await _taskAttachmentsService.MoveToPermanentFilesAsync(userIdHeader, taskId);
        if (result == null) return StatusCode(500);
        return Ok(result);
    }

    [HttpDelete]
    [Route("TEMP/{fileName}")]
    public async Task<ActionResult> DeleteTemporaryFile(string fileName)
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var userIdHeader)) return Unauthorized();
        var result = await _taskAttachmentsService.DeleteTemporaryFileAsync(userIdHeader, fileName);
        if (!result) return StatusCode(500);
        return StatusCode((int)HttpStatusCode.NoContent);; 
    }

    [HttpGet]
    [Route("{taskId}/{fileName}")]
    public async Task<ActionResult> GetPermanentFile(string taskId, string fileName)
    {
        var result = await _taskAttachmentsService.GetTaskAttachment(taskId, fileName);
        if (result == null) return StatusCode(500);
        return File(result.FileStream, result.ContentType, result.FileName, result.LastModified,
            new EntityTagHeaderValue('"' + result.LastModified.ToString() + '"'));
    }
    [HttpGet]
    [Route("{taskId}/Thumbnails/{fileName}")]
    public async Task<ActionResult> GetFileThumbnail(string taskId, string fileName)
    {
        var result = await _taskAttachmentsService.GetTaskAttachmentThumbnail(taskId, fileName);
        if (result == null) return StatusCode(500);
        return File(result.FileStream, result.ContentType, result.FileName, result.LastModified,
            new EntityTagHeaderValue('"' + result.LastModified.ToString() + '"'));
    }

    [HttpDelete]
    [Route("{taskId}/{fileName}")]
    public async Task<ActionResult> DeletePermanentFile(string taskId, string fileName)
    {
        var result = await _taskAttachmentsService.DeletePermanentFileAsync(taskId, fileName);
        if (!result) return StatusCode(500);
        return StatusCode((int)HttpStatusCode.NoContent);
    }
}