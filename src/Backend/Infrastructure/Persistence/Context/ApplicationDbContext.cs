using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Persistence.Context
{
    public class ApplicationDbContext//: DbContext
    {
        public readonly IMongoDatabase _database;

        public ApplicationDbContext()
        {
            var client = new MongoClient("mongodb+srv://fatihmalyemezenerya:fpIM85669huxTeNq@cluster0.bnmmkpg.mongodb.net/AppDb?retryWrites=true&w=majority&appName=Cluster0");
            _database = client.GetDatabase("AppDb");
        }

        public ApplicationDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient("mongodb+srv://fatihmalyemezenerya:fpIM85669huxTeNq@cluster0.bnmmkpg.mongodb.net/AppDb?retryWrites=true&w=majority&appName=Cluster0");
            _database = client.GetDatabase("AppDb");
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