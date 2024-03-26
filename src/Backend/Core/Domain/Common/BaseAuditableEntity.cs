using Domain.Common.Interfaces;

namespace Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
    {
        /*[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("DocumentId")] // _id alanını DocumentId olarak değiştirir
        public string DocumentId { get; set; }*/
        
        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }


        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
