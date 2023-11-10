using Shked.UserDAL.Entities;

namespace Shked.UserDAL.Infrastructure;
/// <summary>
/// Представляет реализацию паттерна репозиторий для коллекции пользователей
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Получения сущности пользователя по его UserId
    /// </summary>
    /// <param name="id">UserId пользователя</param>
    /// <returns>Возвращает NULL, если пользователя с данным UserId не существует</returns>
    Task<User?> GetById(string id);
    /// <summary>
    /// Получение сущности пользователя по его email
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Возвращает NULL, если пользователя с данным email не существует</returns>
    Task<User?> GetByEmail(string email);
    /// <summary>
    /// Создание нового пользователя
    /// </summary>
    /// <param name="newUser"></param>
    /// <returns></returns>
    Task Create(User newUser);
    /// <summary>
    /// Обновление данных пользователя
    /// </summary>
    /// <param name="newUser"></param>
    /// <returns></returns>
    Task<bool> Update(User newUser);
    /// <summary>
    /// Удаление пользователя из коллекции
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> Delete(string id);
}