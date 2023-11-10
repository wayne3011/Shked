using Microsoft.Extensions.Options;

namespace SkedScheduleParser.Application.Handlers.Options;

public class KafkaOptions
{
    public string BootstrapServer { get; set; }
    public string SchedulesTopic { get; set; }
}