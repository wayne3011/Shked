using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SkedAuthoriztion.DAL.DbContext.Options;
using SkedAuthoriztion.DAL.Entities;
using SkedAuthoriztion.DAL.Infrastructure;

namespace SkedAuthoriztion.DAL.DbContext;

public class MongoUserDbContext : IUserDbContext
{
    public MongoUserDbContext(IOptions<MongoOptions> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        Users = database.GetCollection<User>(settings.Value.CollectionName);
    }
    public IMongoCollection<User> Users { get; }
}