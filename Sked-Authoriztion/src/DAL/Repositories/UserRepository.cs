using MongoDB.Bson;
using MongoDB.Driver;
using SkedAuthoriztion.DAL.Entities;
using SkedAuthoriztion.DAL.Infrastructure;

namespace SkedAuthoriztion.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IUserDbContext _db;
    public UserRepository(IUserDbContext database)
    {
        _db = database;
    }
    public async Task<User> GetById(string id)
    {
        var filter = new BsonDocument() { { "Id", id } };
        var cursor = await _db.Users.FindAsync<User>(filter);
        return await cursor.FirstOrDefaultAsync();
    }

    public Task Create(User newUser)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(User newUser)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(string id)
    {
        throw new NotImplementedException();
    }
}