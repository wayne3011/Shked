using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using Microsoft.Extensions.Options;
using SkedScheduleParser.Application.Infrastructure;
using SkedScheduleParser.Application.Models;
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
    public async Task<GroupNameValidationResult> FormatGroupNameAsync(string groupName)
    {
            string formattedGroupName = String.Empty;
            int facultyNumber;
            int courseNumber;
            string courseNumberString;
            string facultyNumberString;
            //Проверяем на совпадение имя группы общему шаблону
            groupName = groupName.ToLower();
            groupName = Regex.Replace(groupName, "-", "");
            if (!Regex.IsMatch(groupName, "\\w([0-9]{1,2}|и)\\w[0-9]{3}\\w{1,3}[0-9]{2}")) return null;
            //Получаем из имени группы номер факультета и номер курса
            facultyNumberString = Regex.Match(groupName, "[0-9]{1,2}|и").Value;
            if (facultyNumberString == "и")
            {
                facultyNumber = 10;
                formattedGroupName += groupName.Substring(0, 3).ToUpper();
                courseNumberString = Regex.Matches(groupName, "[0-9]").ElementAt(0).Value;
            }
            else
            {
                if (!int.TryParse(facultyNumberString, out facultyNumber)) return null;
                formattedGroupName += Regex.Match(groupName, "\\w[0-9]{1,2}\\w").Value.ToUpper() + "-";  
                courseNumberString = Regex.Matches(groupName, "[0-9]").ElementAt(1).Value;
            }
            courseNumber = int.Parse(courseNumberString);

            if (facultyNumber < 1 || facultyNumber > 12) return null;
            if (courseNumber < 1 || courseNumber > 6) return null;
            formattedGroupName += Regex.Match(groupName, "[0-9]{3}");
            
            string learningProfile = string.Join("",Regex.Matches(groupName, "[а-я]|[a-z]"));
            learningProfile = learningProfile.Substring(2);
            learningProfile = Char.ToUpper(learningProfile[0]) + learningProfile.Substring(1);
            formattedGroupName += learningProfile + "-" + groupName.Substring(groupName.Length - 2);    
            
            var document = await _context.OpenAsync(ScheduleUrl + $"/groups.php?department=Институт+№{facultyNumber}&course={courseNumber}");
            var groupList = document.QuerySelector("body>main>div>div>div.col-lg-8.me-auto.mb-7.mb-lg-0>article>div.tab-content")?.Text().Trim('\n', '\t');
            if (groupList == null) throw new Exception("Invalid scheduleURL");
            if (!Regex.IsMatch(groupList, formattedGroupName)) return new GroupNameValidationResult(formattedGroupName, false);
            return new GroupNameValidationResult(formattedGroupName, true);
    }
    public async Task<Schedule?> GetGroupScheduleAsync(string groupName) 
    {
        Schedule schedule = new Schedule();
        int studyWeekCount = await GetStudyWeekCount(groupName);
        for (int weekDay = 1; weekDay <= studyWeekCount; weekDay++)
        {
            var uri = new Uri(ScheduleUrl + $"/index.php?group={HttpUtility.UrlEncode(groupName)}&week={HttpUtility.UrlEncode(weekDay.ToString())}");
            var document = await OpenDocumentAsync(uri, groupName);
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
                    var _class = new Lesson();
                    
                    string classesNameRow = Regex.Replace( classCard[0].Text(), "[\t\n]", String.Empty);
                    
                    _class.Type = classesType[classesNameRow.Substring(classesNameRow.Length - 2)];
                    _class.Name = classesNameRow.Substring(0, classesNameRow.Length - 2);
                    var classInfoRow = classCard[1].Children;
                    _class.Ordinal = GetOrdinalFromClassesTime(classInfoRow[0].Text());
                    
                    if (classInfoRow.Length == 3)
                    {
                        _class.Lecturer = classInfoRow[1].Text();
                        _class.Location = classInfoRow[2].Text();
                    }
                    else if(classInfoRow.Length == 4)
                    {
                        _class.Lecturer = classInfoRow[1].Text()+"/"+ classInfoRow[2].Text();
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
                    alreadyExist.Dates.Add(date.ToShortDateString());
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
        Uri uri = new Uri(ScheduleUrl + $"/index.php?group={HttpUtility.UrlEncode(groupName)}");
        var document = await OpenDocumentAsync(uri,groupName);
        var studyWeeks = document.QuerySelectorAll("#collapseWeeks>div>div>ul>li");           
        int weekCount = studyWeeks.Length;
        return weekCount;
    }

    private async Task<IDocument> OpenDocumentAsync(Uri uri, string groupName)
    {
        groupName = HttpUtility.UrlEncode(groupName);
        CookieContainer cookies = new CookieContainer();
        cookies.Add(new Cookie("schedule-group-cache", "2.0") { Domain = uri.Host });
        cookies.Add(new Cookie("schedule-st-group", groupName) { Domain = uri.Host });
        HttpClientHandler httpClientHandler = new HttpClientHandler();
        httpClientHandler.CookieContainer = cookies;
        HttpClient httpClient = new HttpClient(httpClientHandler);
        var response = await httpClient.GetAsync(uri);
        var document = await _context.OpenAsync(async r => r.Content(await response.Content.ReadAsStreamAsync()));
        return document;
    }
    private static readonly Dictionary<string, string> classesType = new Dictionary<string, string>()
    {
        //lecture practical laboratory exam
        {"ЛК", "lecture"},
        {"ПЗ", "practical"},
        {"ЛР", "laboratory"},
        {"ЭКЗ", "exam"},
    };
    
    private static string _computeDaysScheduleHashSum(List<Lesson> classes)
    {
        using var mD5 = MD5.Create();
        var classesHashSB = new StringBuilder();
        foreach (var el in classes)
        {
            classesHashSB.Append(el.Ordinal);
            classesHashSB.Append(el.Name);
            classesHashSB.Append(el.Type);
            classesHashSB.Append(el.Lecturer);
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