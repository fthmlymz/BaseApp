using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Domain.Common.Interfaces
{
    public interface IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
