using System.Net;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SkedScheduleParser.Application.Handlers.Options;
using SkedScheduleParser.Application.Models;

namespace SkedScheduleParser.Application.Infrastructure;

public class KafkaProducer : IKafkaProducer
{
    private readonly IOptions<KafkaOptions> _options;
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    public KafkaProducer(IOptions<KafkaOptions> options, ILogger<KafkaProducer> logger)
    {
        _options = options;
        _logger = logger;

        _producer = new ProducerBuilder<Null, string>(new ProducerConfig()
        {
            BootstrapServers = _options.Value.BootstrapServer,
            ClientId = Dns.GetHostName(),
        }).SetErrorHandler((producer, error) =>
        {
            _logger.LogError($"Kafka: {error.Reason}");
        }).Build();

    }

    public async Task<DeliveryResult<Null, string>> SendScheduleAsync(string topic, ParsingResponse parsingResponse)
    {
        var value = JsonSerializer.Serialize(parsingResponse);
        var result = await _producer.ProduceAsync(topic, new Message<Null, string>() { Value = value }); 
        return result;
    }
}