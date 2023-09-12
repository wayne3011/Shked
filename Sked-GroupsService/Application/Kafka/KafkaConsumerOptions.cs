namespace SkedGroupsService.Application.Kafka;

public class KafkaConsumerOptions
{
    public string BootstrapServer { get; set; }
    public string SchedulesOptions { get; set; }
    public string GroupID { get; set; }
}