using SkedGroupsService.Application.Models;

namespace SkedGroupsService.Application.Kafka;

public interface IKafkaConsumer
{
    Task<Schedule> GetScheduleMessage(string topic);
}