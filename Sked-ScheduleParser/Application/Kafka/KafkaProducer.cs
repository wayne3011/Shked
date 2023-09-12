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
                // SaslUsername = "4JXQMUM2YGX7NMJR",
                // SaslPassword = "V97xChvbZqFXqY87WnbidrD/oy5X7j7DVyQUh3xsKNb+XtSeWLeM+YFY+1jwPkFh",
                // SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.Https
            }).Build();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<DeliveryResult<Null, string>> SendScheduleAsync(string topic, Schedule message)
    {
        var value = JsonSerializer.Serialize(message);
        return await _producer.ProduceAsync(topic, new Message<Null, string>() { Value = value }); 
    }
}