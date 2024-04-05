using Domain.Common;

namespace Domain.Entities
{
    public class Company : BaseAuditableEntity
    {
        /*[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }*/

        public required string Name { get; set; }
        public string? Description { get; set; } = string.Empty;


        //public ICollection<Category> Categories { get; set; } = new List<Category>();
        //public ICollection<Brand> Brands { get; set; } = new List<Brand>();
        //public ICollection<TransferOfficier> ? TransferOfficiers { get; set; } = new List<TransferOfficier>();
    }
}
