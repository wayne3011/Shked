using MongoDB.Driver;
using SkedAuthorization.DAL.Entities;

namespace SkedAuthorization.DAL.Infrastructure;

public interface IUserDbContext
{
    IMongoCollection<User> Users { get; }
}