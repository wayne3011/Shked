using System.Net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SkedGroupsService.Application.HttpClients.Options;
using SkedGroupsService.Application.Infrastructure;
using SkedGroupsService.Application.Kafka;
using SkedGroupsService.Application.Models;
using SkedGroupsService.DAL.Infrastructure;
using SkedGroupsService.DAL.Models;


namespace SkedGroupsService.Application.Hubs;

public class GroupHub :  Hub
{
    private readonly IScheduleParserApi _scheduleParserApi;
    private readonly IOptions<ParserApiOptions> _options;
    private readonly ILogger<GroupHub> _logger;
    private readonly IScheduleRepository _scheduleRepository;
    public GroupHub(IOptions<ParserApiOptions> options, IScheduleParserApi scheduleParserApi, ILogger<GroupHub> logger, IScheduleRepository scheduleRepository)
    {
        _options = options;
        _scheduleParserApi = scheduleParserApi;
        _logger = logger;
        _scheduleRepository = scheduleRepository;
    }
    public async Task GroupSchedule(string groupName)
    {
        Schedule schedule;
        try
        {
            schedule = await _scheduleRepository.GetAsync(groupName);
        }
        catch (Exception e)
        {
            _logger.LogError("Database connection failed.\nError: {Message}", e.Message);
            schedule = null;
        }
        if (schedule == null)
        {
            await SendParseApplication(groupName);
            return;
        }
        
        await Clients.Caller.SendAsync("CheckParsingProgress", new ParsingProgress { Status = ParseStatus.Success, Schedule = schedule});
    }

    private async Task SendParseApplication(string groupName)
    {
        var parsingApplication = new ParsingApplication()
        {
            ClientID = Context.ConnectionId,
            GroupName = groupName
        };
        var result = await _scheduleParserApi.GetGroupSchedule(parsingApplication);
        if (result)
        {
            await Clients.Caller.SendAsync("CheckParsingProgress",new ParsingProgress() { Status = ParseStatus.Starting });
        }
        else
        {
            _logger.LogError("Failed to contact ScheduleParserService Host:{Host}", _options.Value.Url);
            await Clients.Caller.SendAsync("CheckParsingProgress",new ParsingProgress() { Status = ParseStatus.InternalError });
            Context.Abort();
        }
    } 
    
}