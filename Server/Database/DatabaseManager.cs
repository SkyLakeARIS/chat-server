using MongoDB.Driver;
using Server.Database.Entities;

namespace Server.Database;

public class DatabaseManager
{
    private static readonly Lazy<DatabaseManager> s_lazy = new(() => new DatabaseManager());
    public static DatabaseManager Instance => s_lazy.Value;

    private readonly IMongoDatabase _database;

    private object _lock;

    public static readonly string AccountCollection = "accounts";
    public static readonly string ChatServerCollection = "chatServer";
    public static readonly string AccountManagementCollection = "accountManagement";
    public static readonly string ServerManagementCollection = "chatServerManagement";
    public static readonly string UIDGenerator = "UIDGenerator";
    public static readonly string ServerIDGenerator = "ServerIDGenerator";


    public DatabaseManager()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        _lock = new object();
        _database = client.GetDatabase("chat");
    }

    public IMongoCollection<T> GetCollection<T>(string collection)
    {
        return _database.GetCollection<T>(collection);
    }

    public long GenerateServerID()
    {
	    lock (_lock)
	    {
		    var chatServerDB = _database.GetCollection<ManagementEntity>(ServerManagementCollection);
		    var IDGenerator = chatServerDB.Find(x => x.managerName.Equals(ServerIDGenerator)).ToList()[0];
		    long newID = IDGenerator.ID++;
		    UpdateResult result = chatServerDB.UpdateOne(x => x.managerName.Equals(IDGenerator.managerName), Builders<ManagementEntity>.Update.Set(x => x.ID, IDGenerator.ID));
		    if (result.MatchedCount < 1)
		    {
			    Console.WriteLine($"[LOG] GenerateServerID : serverID를 생성하는 과정에 문제가 발생했습니다. (DB - ID 업데이트 실패) {DateTime.Now}");
		    }
            return newID;
	    }
    }

    public long GenerateUserID()
    {
	    lock (_lock)
	    {
		    var accountDB = _database.GetCollection<ManagementEntity>(AccountManagementCollection);
		    var IDGenerator = accountDB.Find(x => x.managerName.Equals(UIDGenerator)).ToList()[0];
		    long newID = IDGenerator.ID++;
		    UpdateResult result = accountDB.UpdateOne(x => x.managerName.Equals(IDGenerator.managerName), Builders<ManagementEntity>.Update.Set(x => x.ID, IDGenerator.ID));
		    if (result.MatchedCount < 1)
		    {
			    Console.WriteLine($"[LOG] GenerateUserID : UID를 생성하는 과정에 문제가 발생했습니다. (DB - ID 업데이트 실패) {DateTime.Now}");
		    }
		    return newID;
	    }
    }
}