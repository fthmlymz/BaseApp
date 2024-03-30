using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Persistence.HostedService
{
    public class MongoDbInitializer<T> : IHostedService where T : class
    {
        private readonly string _mongoDbConnectionString;
        private readonly string _databaseName;
        private readonly List<string> _collectionNames;
        private readonly ILogger<MongoDbInitializer<T>> _logger;

        public MongoDbInitializer(string? mongoDbConnectionString, string? databaseName, List<string> collectionNames, ILogger<MongoDbInitializer<T>> logger)
        {
            _mongoDbConnectionString = mongoDbConnectionString ?? throw new ArgumentNullException(nameof(mongoDbConnectionString));
            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            _collectionNames = collectionNames ?? throw new ArgumentNullException(nameof(collectionNames));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var mongoClient = new MongoClient(_mongoDbConnectionString);
                var database = mongoClient.GetDatabase(_databaseName);

                bool databaseExists = await DatabaseExistsAsync(mongoClient, _databaseName);
                /*if (!databaseExists)
                {
                    await database.CreateCollectionAsync("dummy"); // MongoDB'de bir veritabanı oluşturmak için en az bir koleksiyon oluşturmak gerekir
                    _logger.LogInformation($"MongoDB database '{_databaseName}' oluşturuldu.");
                }*/

                foreach (var collectionName in _collectionNames)
                {
                    bool collectionExists = await CollectionExistsAsync(database, collectionName);
                    if (!collectionExists)
                    {
                        await database.CreateCollectionAsync(collectionName);
                        _logger.LogInformation($"MongoDB collection '{collectionName}' created.");
                    }
                }

                _logger.LogInformation("MongoDB database and collections have been successfully prepared.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Hata: {ex.Message}");
                //throw; // Hata durumunda uygulamanın başlatılmasını engellemek için istisnayı yeniden fırlat
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Application has been closed.");
            return Task.CompletedTask;
        }

        private async Task<bool> DatabaseExistsAsync(MongoClient mongoClient, string databaseName)
        {
            var databaseNames = await mongoClient.ListDatabaseNamesAsync();
            return (await databaseNames.ToListAsync()).Contains(databaseName);
        }

        private async Task<bool> CollectionExistsAsync(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = await database.ListCollectionNames().ToListAsync();
            return collections.Contains(collectionName);
        }
    }
}
