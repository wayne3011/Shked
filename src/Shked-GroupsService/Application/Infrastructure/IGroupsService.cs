using ShkedGroupsService.Application.DTO;
using ShkedGroupsService.Application.DTO.ScheduleDTO;

namespace ShkedGroupsService.Application.Infrastructure;
/// <summary>
/// Представляет сервис бизнес-логики, связанной с учебными группами
/// </summary>
public interface IGroupsService
{
    /// <summary>
    /// Получить расписание группы во её имени
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    Task<ScheduleDTO?> GetGroupSchedule(string groupName);
}