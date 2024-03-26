using Domain.Entities;
using Domain.Entities;
using MongoDB.Driver;

namespace Persistence.Collections
{
    public class AppDbCollections
    {
        public IMongoCollection<Company> ? Companies { get; set; }
        public IMongoCollection<Product> ? Product { get; set; }
    }
}
