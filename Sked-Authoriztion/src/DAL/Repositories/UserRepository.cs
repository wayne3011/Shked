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
    public async Task<User?> GetById(string id)
    {
        var filter = new BsonDocument() { { "Id", id } };
        var cursor = await _db.Users.FindAsync<User>(filter);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmail(string email)
    {
        var cursor = await _db.Users.FindAsync(x => x.Email == email);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task Create(User newUser)
    {
        await _db.Users.InsertOneAsync(newUser);
    }

    public async Task<bool> Update(User newUser)
    {
        var replaceResult = await _db.Users.ReplaceOneAsync(x => x.Id == newUser.Id, newUser);
        return replaceResult.IsAcknowledged && replaceResult.ModifiedCount == 1;
    }

    public async Task<bool> Delete(string id)
    {
        var deleteResult = await _db.Users.DeleteOneAsync(x => x.Id == id);
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount == 1;
    }
}