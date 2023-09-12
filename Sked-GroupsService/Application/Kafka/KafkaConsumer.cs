using System.Reactive.Linq;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using SkedGroupsService.Application.Models;

namespace SkedGroupsService.Application.Kafka;

public class KafkaConsumer : IKafkaConsumer
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly IOptions<KafkaConsumerOptions> _options;
    
    public KafkaConsumer(IOptions<KafkaConsumerOptions> options)
    {
        _options = options;
        _consumer = new ConsumerBuilder<Null, string>(new ConsumerConfig()
        {
            BootstrapServers = _options.Value.BootstrapServer,
            GroupId = _options.Value.GroupID,
            AutoOffsetReset = AutoOffsetReset.Earliest
        }).Build();
    }

    public Task<Schedule> GetScheduleMessage(string topic)
    {
        _consumer.Subscribe(topic);
    }
}