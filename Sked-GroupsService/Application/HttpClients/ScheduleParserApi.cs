using System.Net;
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
}