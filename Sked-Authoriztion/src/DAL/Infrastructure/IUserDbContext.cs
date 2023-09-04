using MongoDB.Driver;
using SkedAuthoriztion.DAL.Entities;

namespace SkedAuthoriztion.DAL.Infrastructure;

public interface IUserDbContext
{
    IMongoCollection<User> Users { get; }
}