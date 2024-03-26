using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Domain.Common.Interfaces
{
    public interface IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("DocumentId")] // _id alanını DocumentId olarak değiştirir
        public string Id { get; set; }
    }
}
