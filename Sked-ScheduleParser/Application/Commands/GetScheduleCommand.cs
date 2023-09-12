using MediatR;

namespace SkedScheduleParser.Application.Commands;

public record GetScheduleCommand(string groupName) : IRequest;
