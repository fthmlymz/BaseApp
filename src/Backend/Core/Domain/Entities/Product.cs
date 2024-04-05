using Domain.Common;

namespace Domain.Entities
{
    public class Product : BaseAuditableEntity
    {
        public required string ProductName { get; set; }
        public string? ProductDescription { get; set; }
    }
}
