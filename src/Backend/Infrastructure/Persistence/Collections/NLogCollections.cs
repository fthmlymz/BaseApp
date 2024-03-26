using Domain.Entities;
using MongoDB.Driver;

namespace Persistence.Collections
{
    public class NLogCollections
    {
        public IMongoCollection<Company> NlogCompany { get; private set; }

    }
}
