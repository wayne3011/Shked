namespace SkedAuthoriztion.DAL.Infrastructure;

public interface IUserCollectionSettings 
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }
}