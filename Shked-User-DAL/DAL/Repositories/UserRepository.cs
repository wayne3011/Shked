using MongoDB.Bson;
using MongoDB.Driver;
using Shked.UserDAL.Entities;
using Shked.UserDAL.Infrastructure;


namespace Shked.UserDAL.Repositories;
/// <summary>
/// Представляет реализацию коллекции пользователей <see cref="IUserRepository"/>
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IUserDbContext _db;

    public UserRepository(IUserDbContext database)
    {
        _db = database;
    }
    public async Task<User?> GetById(string id)
    {
        var cursor = await _db.Users.FindAsync<User>(x => x.Id == id);
        return (await cursor.ToListAsync()).FirstOrDefault();
    }

    public async Task<User?> GetByEmail(string email)
    {
        var cursor = await _db.Users.FindAsync(x => x.Email == email);
        return (await cursor.ToListAsync()).FirstOrDefault();
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