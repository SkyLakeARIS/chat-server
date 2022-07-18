using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Database.Entities
{
    public sealed class ChatServerEntity : IMongoEntity
    {
	    [BsonId]
	    public ObjectId entityId { get; set; }

	    public long serverID { get; init; }

	    public string serverName { get; init; }

	    public List<long> userList { get; init; }

    }
}
