using AccountType;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Database.Entities
{
    public sealed class ChatServerEntity : IMongoEntity
    {
	    [BsonId]
	    public ObjectId entityId { get; set; }

	    public long serverID { get; init; }

	    public long ownerID { get; init; }

		public string serverName { get; init; }

	    public List<long> jointedUserList { get; init; }
    }
}
