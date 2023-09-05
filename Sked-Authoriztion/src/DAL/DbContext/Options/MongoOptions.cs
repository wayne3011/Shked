namespace SkedAuthoriztion.DAL.DbContext.Options;

public class MongoOptions
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }
}