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
    public KafkaProducer(IOptions<KafkaOptions> options)
    {
        _options = options;
        try
        {
            _producer = new ProducerBuilder<Null, string>(new ProducerConfig()
            {
                BootstrapServers = _options.Value.BootstrapServer,
                ClientId = Dns.GetHostName(),
            }).Build();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<DeliveryResult<Null, string>> SendScheduleAsync(string topic, ParsingResponse parsingResponse)
    {
        var value = JsonSerializer.Serialize(parsingResponse);
        var result = await _producer.ProduceAsync(topic, new Message<Null, string>() { Value = value }); 
        return result;
    }
}