using AccountType;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Database.Entities;


public sealed record AccountEntity : IMongoEntity
{
    [BsonId]
    public ObjectId entityId { get; set; }

    public long UID { get; init; }

    public string ID { get; init; }

    public string password { get; init; }

    public string nickName { get; init; }

    public List<long> joinedServerList { get; init; }

    public List<EAccountType> accountTypeList { get; init; }

}
