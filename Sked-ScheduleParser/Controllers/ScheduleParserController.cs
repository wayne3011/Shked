using MediatR;
using Microsoft.AspNetCore.Mvc;
using SkedScheduleParser.Application.Commands;
using SkedScheduleParser.Application.Infrastructure;
using SkedScheduleParser.Application.Models;

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
    [Route("/GetSchedule")]
    public IActionResult GetSchedule([FromQuery] ParsingApplication parsingApplication)
    {
        _mediator.Send(new GetScheduleCommand(parsingApplication)).RunSynchronously();
        return Ok();
    }

    [HttpGet]
    [Route("/FormatGroupName")]
    public async Task FormatGroupName([FromQuery] string groupName)
    {
        return await _mediator.Send<GroupNameValidationResult>(new FormatGroupNameCommand(groupName));
    }

}