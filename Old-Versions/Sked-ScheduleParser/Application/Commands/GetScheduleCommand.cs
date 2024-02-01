using MediatR;
using SkedScheduleParser.Application.Models;

namespace SkedScheduleParser.Application.Commands;

public record GetScheduleCommand(ParsingApplication parsingApplication) : IRequest;
