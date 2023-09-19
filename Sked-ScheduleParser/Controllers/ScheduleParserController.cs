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
    public async Task<IActionResult> GetSchedule([FromQuery] ParsingApplication parsingApplication)
    {
        _mediator.Send(new GetScheduleCommand(parsingApplication));
        return Ok();
    }

}