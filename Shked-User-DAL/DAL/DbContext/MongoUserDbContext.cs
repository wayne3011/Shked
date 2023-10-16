using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shked.UserDAL.DbContext.Options;
using Shked.UserDAL.Entities;
using Shked.UserDAL.Infrastructure;

namespace Shked.UserDAL.DbContext;

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