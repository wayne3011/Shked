using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Options;
using SkedScheduleParser.Application.Commands;
using SkedScheduleParser.Application.Handlers.Options;
using SkedScheduleParser.Application.Infrastructure;
using SkedScheduleParser.Application.Models;

namespace SkedScheduleParser.Application.Handlers;

public class GetScheduleHandler : IRequestHandler<GetScheduleCommand>
{
    readonly IScheduleParserService _scheduleParserService;
    private readonly IOptions<KafkaOptions> _kafkaOptions;
    private readonly IKafkaProducer _kafkaProducer;
    public GetScheduleHandler(IScheduleParserService scheduleParserService, IOptions<KafkaOptions> kafkaOptions, IKafkaProducer kafkaProducer)
    {
        _scheduleParserService = scheduleParserService;
        _kafkaOptions = kafkaOptions;
        try
        {
            _kafkaProducer = kafkaProducer;

        }
        catch (Exception )
        {
            Console.WriteLine();
            throw;
        }
        
    }
    public async Task Handle(GetScheduleCommand request, CancellationToken cancellationToken)
    {

        var schedule = await _scheduleParserService.GetGroupScheduleAsync(request.parsingApplication.GroupName);
        await _kafkaProducer.SendScheduleAsync(_kafkaOptions.Value.SchedulesTopic, new ParsingResponse()
        {
            ClientID = request.parsingApplication.ClientID,
            NewSchedule = schedule
        });

    }
}