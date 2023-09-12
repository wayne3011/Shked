using Confluent.Kafka;
using SkedScheduleParser.Application.Models;

namespace SkedScheduleParser.Application.Infrastructure;

public interface IKafkaProducer
{
    Task<DeliveryResult<Null, string>> SendScheduleAsync(string topic, Schedule message); 
}