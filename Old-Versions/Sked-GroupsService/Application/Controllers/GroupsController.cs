using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using SkedGroupsService.Application.HttpClients;
using SkedGroupsService.Application.Infrastructure;
using SkedGroupsService.Application.Models;

namespace SkedGroupsService.Application.Controllers;
[Route("API/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IScheduleParserApi _scheduleParserApi;
    private readonly ILogger<GroupsController> _logger;
    public GroupsController(IScheduleParserApi scheduleParserApi, ILogger<GroupsController> logger)
    {
        _scheduleParserApi = scheduleParserApi;
        _logger = logger;
    }
    [HttpGet]
    [Route("/FormatGroupName/{groupName}")]
    public async Task<ActionResult<GroupNameValidationResult>> FormatGroupName(string groupName)
    {
        if (string.IsNullOrEmpty(groupName)) return BadRequest();
        try
        {
            await _scheduleParserApi.FormatGroupName(groupName);
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
        return Ok();
    }
}