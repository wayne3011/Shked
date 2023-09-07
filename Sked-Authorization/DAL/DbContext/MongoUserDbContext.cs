using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SkedAuthorization.DAL.DbContext.Options;
using SkedAuthorization.DAL.Entities;
using SkedAuthorization.DAL.Infrastructure;

namespace SkedAuthorization.DAL.DbContext;

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