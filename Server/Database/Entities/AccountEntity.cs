using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Database.Entities;

public sealed record AccountEntity : IMongoEntity
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string UserName { get; init; }

    public string Password { get; init; }

    public string NickName { get; init; }

    public string Email { get; init; }

    public DateTime RegisterDate { get; init; }

    public int AccountType { get; init; }
}