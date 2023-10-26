using ShkedGroupsService.DAL.Models;

namespace ShkedGroupsService.DAL.Infrastructure;
/// <summary>
/// Представляет паттерн репозитария для коллекции учебных групп
/// </summary>
public interface IScheduleRepository
{
    /// <summary>
    /// Получение расписания группы по её имени
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns>В случае отсутствия расписания в коллекции возвращает NULL</returns>
    Task<Schedule?> GetAsync(string groupName);
    /// <summary>
    /// Добавление нового расписания в коллекцию
    /// </summary>
    /// <param name="newSchedule"></param>
    /// <returns>Возвращает true, если расписание было добавлено</returns>
    Task<bool> CreateAsync(Schedule newSchedule);
    /// <summary>
    /// Обновление расписания в коллекции
    /// </summary>
    /// <param name="schedule"></param>
    /// <returns>Возвращает true в случае удачного обновления расписания</returns>
    Task<bool> UpdateAsync(Schedule schedule);
}