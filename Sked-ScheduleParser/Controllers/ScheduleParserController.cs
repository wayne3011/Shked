using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkedScheduleParser.Application.Commands;
using SkedScheduleParser.Application.Infrastructure;

namespace SkedScheduleParser.Controllers;
[Route("API/ScheduleParser")]
public class ScheduleParserController : Controller
{
    private readonly IMediator _mediator;

    public ScheduleParserController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    [Route("/GetSchedule/{groupName}")]
    public async Task<IActionResult> GetSchedule(string groupName)
    {
        _mediator.Send(new GetScheduleCommand(groupName));
        return Ok();
    }

}