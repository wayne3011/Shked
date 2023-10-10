using Microsoft.AspNetCore.Mvc;
using ShkedGroupsService.Application.DTO.ScheduleDTO;
using ShkedGroupsService.Application.Infrastructure;

namespace ShkedGroupsService.UI.Controllers;
[Route("API/[controller]/")]
public class GroupsController : ControllerBase
{
    private readonly IGroupsService _groupsService;
    
    public GroupsController(IGroupsService groupsService)
    {
        _groupsService = groupsService;
    }
    [HttpGet]
    [Route("{groupName}")]
    public async Task<ScheduleDTO> GetAsync([FromRoute]string groupName)
    {
        var temp = await _groupsService.GetGroupSchedule(groupName);
        return temp;
    }
}