using MongoDB.Driver;
using SkedAuthoriztion.DAL.Entities;
using SkedAuthoriztion.DAL.Infrastructure;

namespace SkedAuthoriztion.DAL.DbContext;

public class MongoUserDbContext : IUserDbContext
{
    public MongoUserDbContext(IUserCollectionSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        Users = database.GetCollection<User>(settings.CollectionName);
    }
    public IMongoCollection<User> Users { get; }
}