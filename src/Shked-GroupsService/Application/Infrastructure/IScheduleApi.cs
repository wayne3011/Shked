using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.Application.Infrastructure;
/// <summary>
/// Представляет надстройку над API ВУЗа для получения расписания в необходимой форме
/// </summary>
public interface IScheduleApi
{
    /// <summary>
    /// Получить расписание из API ВУЗа
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    Task<Schedule?> GetScheduleAsync(string groupName);
}