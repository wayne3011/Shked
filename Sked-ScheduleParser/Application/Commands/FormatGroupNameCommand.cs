using MediatR;
using SkedScheduleParser.Application.Models;

namespace SkedScheduleParser.Application.Commands;

public record FormatGroupNameCommand(string groupName) : IRequest<GroupNameValidationResult>;



