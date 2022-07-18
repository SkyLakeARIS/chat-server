using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Database.Entities;

public sealed record ManagementEntity : IMongoEntity
{
	[BsonId]
	public ObjectId entityId { get; set; }

	public long UID;

	public string managerName;
}

