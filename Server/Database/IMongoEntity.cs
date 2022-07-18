using MongoDB.Bson;

namespace Server.Database
{
    public interface IMongoEntity
    {
        ObjectId entityId { get; set; }
    }
}