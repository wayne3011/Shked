using System.Text.Json;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SkedGroupsService.Application.Extensions.JsonConverters;
using SkedGroupsService.Application.Hubs;
using SkedGroupsService.Application.Models;
using SkedGroupsService.DAL.Infrastructure;
using SkedScheduleParser.Application.Models;

namespace SkedGroupsService.Application.Kafka;

public class KafkaConsumerHostedService : BackgroundService
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly IOptions<KafkaConsumerOptions> _options;
    private readonly IHubContext<GroupHub> _hubContext;
    private readonly ILogger<KafkaConsumerHostedService> _logger;
    private readonly IScheduleRepository _scheduleRepository;
    

    public KafkaConsumerHostedService(IOptions<KafkaConsumerOptions> options, IHubContext<GroupHub> hubContext, ILogger<KafkaConsumerHostedService> logger, IScheduleRepository scheduleRepository)
    {
        _options = options;
        _hubContext = hubContext;
        _logger = logger;
        _scheduleRepository = scheduleRepository;
        _consumer = new ConsumerBuilder<Null, string>(new ConsumerConfig()
        {
            BootstrapServers = _options.Value.BootstrapServer,
            GroupId = _options.Value.GroupID,
            AutoOffsetReset = AutoOffsetReset.Earliest
        }).SetErrorHandler((consumer, ex) =>
        {
            _logger.LogError("Kafka: {Message}", ex.Reason);
        }).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Run((() => PollMessages(stoppingToken)), cancellationToken: stoppingToken);
        }
    }

    private async Task PollMessages(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_options.Value.SchedulesTopic);
        while (!stoppingToken.IsCancellationRequested)
        {
            var newMessage = _consumer.Consume(stoppingToken);
            
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new DateOnlyJsonConverter());
            var parsingResponse = JsonSerializer.Deserialize<ParsingResponse>(newMessage.Message.Value,serializeOptions); 
            
            if (parsingResponse == null)
            {
                await _hubContext.Clients.Client(parsingResponse.ClientID)
                    .SendAsync("CheckParsingProgress", 
                        new ParsingProgress() { Status = ParseStatus.InternalError}, cancellationToken: stoppingToken);
                _logger.LogError("Incorrect response from ScheduleParserService:\n{@Response}",newMessage);
                return;
            }
            
            try
            {
                if(parsingResponse.NewSchedule != null) await _scheduleRepository.CreateAsync(parsingResponse.NewSchedule);
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to save the schedule object to the database.\nError: {Error}", e.Message);
            }
            
            await _hubContext.Clients.Client(parsingResponse.ClientID)
                .SendAsync("CheckParsingProgress", 
                    new ParsingProgress()
                    {
                        Status = ParseStatus.Ended, 
                        Schedule = parsingResponse.NewSchedule,
                    }, cancellationToken: stoppingToken);
        }
    }
}