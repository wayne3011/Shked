using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using SkedScheduleParser.Application.Infrastructure;
using SkedScheduleParser.Application.Models;
using SkedScheduleParser.Application.Services.Extensions;
using SkedScheduleParser.Application.Services.Options;

namespace SkedScheduleParser.Application.Services;

public class ScheduleParserService : IScheduleParserService
{
    private string ScheduleUrl { get; set; }
    private IBrowsingContext _context { get; set; } = BrowsingContext.New(new Configuration().WithDefaultLoader());
    public ScheduleParserService(IOptions<ScheduleParserOptions> options)
    {
        ScheduleUrl = options.Value.MAIScheduleUrl;
    } 
    public Task<string?> FormatGroupNameAsync(string groupName)
    {
        throw new NotImplementedException();
    }
    public async Task<Schedule?> GetGroupScheduleAsync(string groupName) 
    {
        Schedule schedule = new Schedule();
        int studyWeekCount = await GetStudyWeekCount(groupName);
        for (int weekDay = 1; weekDay <= studyWeekCount; weekDay++)
        {
            string url = ($"/index.php?group={HttpUtility.UrlEncode(groupName)}&week={HttpUtility.UrlEncode(weekDay.ToString())}");
            var document = await _context.OpenAsync(ScheduleUrl + url);
            if (document == null) return null;
                //throw new BrokenWebSiteConnectionException(ScheduleUrl,groupName);
            var dayCards = document.QuerySelectorAll("body>main>div>div>div.col-lg-8.me-auto.mb-7.mb-lg-0>article>ul>li>div>div");
            //TODO: WARNING DESIGN CHANGED!!!
            if (dayCards.Length == 0)
            {
                var isHaveSchedule = document.QuerySelector("body>main>div>div>div.col-lg-8.me-auto.mb-7.mb-lg-0>article>div.w-md-75.w-xl-50.text-center.mx-md-auto.mb-5.mb-md-9>img") is not null;
                if (!isHaveSchedule) return null;
//                    throw new AbsenceScheduleObjectsException(ScheduleUrl,groupName);
            }
            
            foreach (var dayCard in dayCards)//parse study day
            {
               
                DailySchedule daysSchedule = new DailySchedule();
                
                DateTime date = Convert.ToDateTime(dayCard.FirstElementChild.Text().Trim(new char[] { '\n', '\t' }).Remove(0, 4));
                daysSchedule.Dates.Add(date.ToShortDateString());        
                
                var dayCardElements = dayCard.Children.Where(el => el.TagName == "DIV");//get schoolday schedule
                foreach (var el in dayCardElements)//for each classes parse it
                {
                    var classCard = el.Children;
                    var _class = new Class();
                    
                    string classesNameRow = Regex.Replace( classCard[0].Text(), "[\t\n]", String.Empty);
                    
                    _class.Type = classesType[classesNameRow.Substring(classesNameRow.Length - 2)];
                    _class.Name = classesNameRow.Substring(0, classesNameRow.Length - 2);
                    var classInfoRow = classCard[1].Children;
                    _class.Ordinal = GetOrdinalFromClassesTime(classInfoRow[0].Text());
                    
                    if (classInfoRow.Length == 3)
                    {
                        _class.Teacher = classInfoRow[1].Text();
                        _class.Location = classInfoRow[2].Text();
                    }
                    else if(classInfoRow.Length == 4)
                    {
                        _class.Teacher = classInfoRow[1].Text()+"/"+ classInfoRow[2].Text();
                        _class.Location = classInfoRow[3].Text();
                    }
                    else
                    {
                        _class.Location = classInfoRow[1].Text();
                    }
                    
                    daysSchedule.Classes.Add(_class);
    
                }
                
                daysSchedule.HashSum = _computeDaysScheduleHashSum(daysSchedule.Classes);
                
                var weekday = schedule.Week[(int)date.DayOfWeek - 1];
                var alreadyExist = weekday.DaysSchedules.FirstOrDefault(x => x == daysSchedule);
                if (alreadyExist != null)
                {
                    alreadyExist.Dates.Add(daysSchedule.Dates.First());
                }
                else
                {
                    weekday.DaysSchedules.Add(daysSchedule);
                }
            }
        }
        
        return schedule;
    }
    private async Task<int> GetStudyWeekCount(string groupName)
    {
        var document = await _context.OpenAsync(ScheduleUrl + $"?group={groupName}");
        var studyWeeks = document.QuerySelectorAll("#collapseWeeks>div>div>ul>li");           
        int weekCount = studyWeeks.Length;
        return weekCount;
    }
    
    private static readonly Dictionary<string, string> classesType = new Dictionary<string, string>()
    {
        //lecture practical laboratory exam
        {"ЛК", "lecture"},
        {"ПЗ", "practical"},
        {"ЛР", "laboratory"},
        {"ЭКЗ", "exam"},
    };
    
    private static string _computeDaysScheduleHashSum(List<Class> classes)
    {
        using var mD5 = MD5.Create();
        var classesHashSB = new StringBuilder();
        foreach (var el in classes)
        {
            classesHashSB.Append(el.Ordinal);
            classesHashSB.Append(el.Name);
            classesHashSB.Append(el.Type);
            classesHashSB.Append(el.Teacher);
            classesHashSB.Append(el.Location);
        }
        return Convert.ToHexString(mD5.ComputeHash(Encoding.UTF8.GetBytes(classesHashSB.ToString())));
    }
    
    private static int GetOrdinalFromClassesTime(string time)
    {
        switch (time)
        {
            case "09:00 – 10:30":
                return 1;
            case "10:45 – 12:15":
                return 2;
            case "13:00 – 14:30":
                return 3;
            case "14:45 – 16:15":
                return 4;
            case "16:30 – 18:00":
                return 5;
            case "18:15 – 19:45":
                return 6;
            case "20:00 – 21:30":
                return 7;
            default:
                throw new Exception("Invalid class time");
        }
    }
}