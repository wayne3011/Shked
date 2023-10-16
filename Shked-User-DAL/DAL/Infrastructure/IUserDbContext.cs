using MongoDB.Driver;
using Shked.UserDAL.Entities;

namespace Shked.UserDAL.Infrastructure;

public interface IUserDbContext
{
    IMongoCollection<User> Users { get; }
}