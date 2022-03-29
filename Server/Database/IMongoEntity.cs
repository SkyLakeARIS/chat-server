using MongoDB.Bson;

namespace Server.Database
{
    public interface IMongoEntity
    {
        ObjectId Id { get; set; }
    }
}

/*--------------------
	
 --------------------*/