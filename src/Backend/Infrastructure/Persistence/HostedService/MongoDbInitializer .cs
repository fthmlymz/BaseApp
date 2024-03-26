using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection;

namespace Persistence.HostedService
{
    public class MongoDbInitializer<T> : IHostedService where T : class
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;
        private readonly MongoUrl _mongoUrl;

        public MongoDbInitializer(MongoUrl mongoUrl, IMongoDatabase database, IMongoCollection<T> collection)
        {
            _database = database;
            _collection = collection;
            _mongoUrl = mongoUrl;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("MongoDbInitializer is starting.");

            var mongoUrl = new MongoUrl(_mongoUrl.ToString());
            var username = mongoUrl.Username;
            var password = mongoUrl.Password;

            // Kimlik doğrulama bilgileriyle yeni bir MongoClient oluştur
            var settings = MongoClientSettings.FromUrl(mongoUrl);
            settings.Credential = MongoCredential.CreateCredential(mongoUrl.DatabaseName, username, password);
            var mongoClient = new MongoClient(settings);

            var database = mongoClient.GetDatabase(_database.DatabaseNamespace.DatabaseName);

            if (!await DatabaseExistsAsync(database))
            {
                await DatabaseExistsAsync(database);
            }
            await CreateCollectionsAsync(database);

            //_logger.LogInformation("MongoDbInitializer has finished its work.");
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task<bool> DatabaseExistsAsync(IMongoDatabase database)
        {
            var databaseNames = await database.Client.ListDatabaseNames().ToListAsync();
            return databaseNames.Contains(database.DatabaseNamespace.DatabaseName);
        }

        private async Task CreateCollectionsAsync(IMongoDatabase database)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(IMongoCollection<>));

            foreach (var property in properties)
            {
                var collectionType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                if (collectionType != null)
                {
                    var collectionName = collectionType.Name + "s";
                    if (!await CollectionExists(database, collectionName))
                    {
                        await database.CreateCollectionAsync(collectionName);
                        // _logger.LogInformation($"Collection '{collectionName}' created.");
                    }
                }
            }
        }
        private async Task<bool> CollectionExists(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = await database.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            return await collections.AnyAsync();
        }
    }
}
