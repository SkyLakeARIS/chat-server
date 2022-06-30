using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Database.Entities;

public enum EAccountType : byte
{
    User,
    Host,
    Admin
}

public sealed record AccountEntity : IMongoEntity
{
    [BsonId]
    public ObjectId entityId { get; set; }

    public string ID { get; init; }

    public string password { get; init; }

    public string nickName { get; init; }

    public EAccountType accountType { get; init; }
}