using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Persistence.Context
{
    public class ApplicationDbContext
    {
        public readonly IMongoDatabase _database;

        public ApplicationDbContext()
        {

        }
        //public ApplicationDbContext(IConfiguration configuration)
        //{
        //    var databasesSection = configuration.GetSection("Databases");
        //    var applicationDatabaseName = databasesSection["ApplicationDatabaseName"];

        //    var connectionString = configuration.GetConnectionString("ApplicationDbConnection");
        //    var client = new MongoClient(connectionString);
        //    _database = client.GetDatabase(applicationDatabaseName);
        //}
        public IMongoCollection<Company> Companies { get; private set; }
    }
}



//public IMongoCollection<Company> Companies => _database.GetCollection<Company>("Companies");
/*var mongoDbSettings = configuration.GetSection("MongoDbSettings");
var connectionString = $"mongodb://{mongoDbSettings["Username"]}:{mongoDbSettings["Password"]}@{mongoDbSettings["Hostname"]}/{mongoDbSettings["DatabaseName"]}?authMechanism={mongoDbSettings["authMechanism"]}";
var settings = MongoClientSettings.FromConnectionString(connectionString);
var mongoClient = new MongoClient(settings);
_database = mongoClient.GetDatabase(mongoDbSettings["DatabaseName"]);
*/