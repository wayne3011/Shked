namespace ShkedGroupsService.Application.Sources;

public class DailyScheduleApiModel
{
    public Dictionary<string, Dictionary<string, ClassApiModel>> pairs { get; set; }
}

public class ClassApiModel
{
    public Dictionary<string, string> lector { get; set; }
    public Dictionary<string,int> type { get; set; }
    public Dictionary<string,string> room { get; set; }
}