using System.Reactive.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SkedGroupsService.Application.Hubs;
using SkedGroupsService.Application.Models;
using SkedScheduleParser.Application.Models;

namespace SkedGroupsService.Application.Kafka;

public class KafkaConsumerHostedService : BackgroundService
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly IOptions<KafkaConsumerOptions> _options;
    private readonly IHubContext<GroupHub> _hubContext;
    private readonly ILogger<KafkaConsumerHostedService> _logger;
    

    public KafkaConsumerHostedService(IOptions<KafkaConsumerOptions> options, IHubContext<GroupHub> hubContext, ILogger<KafkaConsumerHostedService> logger)
    {
        _options = options;
        _hubContext = hubContext;
        _logger = logger;
        _consumer = new ConsumerBuilder<Null, string>(new ConsumerConfig()
        {
            BootstrapServers = _options.Value.BootstrapServer,
            GroupId = _options.Value.GroupID,
            AutoOffsetReset = AutoOffsetReset.Earliest
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
            //var parsingResponse = JsonSerializer.Deserialize<ParsingResponse>(newMessage.Message.Value);
            var parsingResponse = JsonConvert.DeserializeObject<ParsingResponse>(newMessage.Message.Value); 
            if (parsingResponse == null)
            {
                await _hubContext.Clients.Client(parsingResponse.ClientID)
                    .SendAsync("CheckParsingProgress", 
                        new ParsingProgress() { Status = ParseStatus.InternalError}, cancellationToken: stoppingToken);
                _logger.LogError("Incorrect response from ScheduleParserService:\n{@Response}",newMessage);
            }
            await _hubContext.Clients.Client(parsingResponse.ClientID)
                .SendAsync("CheckParsingProgress", 
                    new ParsingProgress()
                    {
                        Status = ParseStatus.Ended, Schedule = parsingResponse.NewSchedule
                    }, cancellationToken: stoppingToken);
        }
    }
}