using MongoDB.Driver;
using Shked.UserDAL.Entities;

namespace Shked.UserDAL.Infrastructure;
/// <summary>
/// Представляет контекст БД пользователей
/// </summary>
public interface IUserDbContext
{
    /// <summary>
    /// Коллекция пользователей
    /// </summary>
    IMongoCollection<User> Users { get; }
}