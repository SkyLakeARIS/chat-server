using MongoDB.Driver;

namespace Server.Database;

public class DatabaseManager
{
    private static readonly Lazy<DatabaseManager> Lazy = new(() => new DatabaseManager());
    public static DatabaseManager Instance => Lazy.Value;

    private readonly IMongoDatabase _database;

    public static readonly string AccountCollection = "accounts";
    public static readonly string ChatServerCollection = "chatServer";
    public static readonly string AccountManagement = "accountManagement";
    public static readonly string UIDGenerator = "UIDGenerator";

    public DatabaseManager()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        _database = client.GetDatabase("chat");
    }

    public IMongoCollection<T> GetCollection<T>(string collection)
    {
        return _database.GetCollection<T>(collection);
    }
}