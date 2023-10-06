using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SkedGroupsService.Application.HttpClients.Options;
using SkedGroupsService.Application.Infrastructure;
using SkedGroupsService.Application.Models;
using SkedScheduleParser.Application.Models;

namespace SkedGroupsService.Application.HttpClients;

public class ScheduleParserApi : IScheduleParserApi
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<ParserApiOptions> _options;
    public ScheduleParserApi(IOptions<ParserApiOptions> options)
    {
        _options = options;
        _httpClient = new HttpClient();
    }
    public async Task<bool> GetGroupSchedule(ParsingApplication parsingApplication)
    {
        var requestUri = new Uri(_options.Value.Url + 
                                 _options.Value.GetGroupSchedule + 
                                 $"?ClientID={parsingApplication.ClientID}&GroupName={parsingApplication.GroupName}");
        var response = await _httpClient.GetAsync(requestUri);
        return response.StatusCode == HttpStatusCode.OK;
    }

    public async Task<GroupNameValidationResult?> FormatGroupName(string groupName)
    {
        var requestUri = new Uri(_options.Value.Url + _options.Value.FormatGroupName + $"groupName={groupName}");
        var response = await _httpClient.GetAsync(requestUri);
        var resultString= await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GroupNameValidationResult>(resultString);
    }
}