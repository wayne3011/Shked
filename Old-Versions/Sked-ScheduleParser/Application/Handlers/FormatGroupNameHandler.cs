using MediatR;
using SkedScheduleParser.Application.Commands;
using SkedScheduleParser.Application.Infrastructure;
using SkedScheduleParser.Application.Models;

namespace SkedScheduleParser.Application.Handlers;

public class FormatGroupNameHandler : IRequestHandler<FormatGroupNameCommand, GroupNameValidationResult>
{
    private readonly IScheduleParserService _scheduleParserService;
    public FormatGroupNameHandler(IScheduleParserService scheduleParserService)
    {
        _scheduleParserService = scheduleParserService;
    }
    public async Task<GroupNameValidationResult> Handle(FormatGroupNameCommand request, CancellationToken cancellationToken)
    {
        return await _scheduleParserService.FormatGroupNameAsync(request.groupName);
    }
}