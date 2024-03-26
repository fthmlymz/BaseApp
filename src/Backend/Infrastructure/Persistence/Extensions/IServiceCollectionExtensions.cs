using Application.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Persistence.Context;
using Persistence.HostedService;
using Persistence.Repositories;
using Serilog;
using Serilog.Events;

namespace Persistence.Extensions
{
    public static class IServiceCollectionExtensions
    {

        public static IServiceCollection AddPersistenceLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepositories();
            AddDbContexts(services, configuration);

            return services;
        }


        private static void AddDbContexts(IServiceCollection services, IConfiguration configuration)
        {
            #region Application Collections&Configuration
            //services.AddMongoDbInitializer<AppDbCollections>(
            //       configuration.GetConnectionString("ApplicationDbConnection"),
            //       configuration["Databases:ApplicationDbName"]
            //   );
            #endregion


            #region EventBus - DotnetCore Cap MongoDB
            services.AddCap(x =>
            {
                x.UseMongoDB(opt =>
                {
                    opt.DatabaseConnection = configuration.GetConnectionString("DotnetCoreCapConnection");
                    //opt.DatabaseName = configuration["Databases:DotnetCoreCapDbName"];
                });
                x.UseRabbitMQ(options =>
                {
                    options.ExchangeName = "BaseStations";
                    options.BasicQosOptions = new DotNetCore.CAP.RabbitMQOptions.BasicQos(3);
                    options.ConnectionFactoryOptions = opt =>
                    {
                        opt.HostName = configuration.GetSection("RabbitMQ:Host").Value;
                        opt.UserName = configuration.GetSection("RabbitMQ:Username").Value;
                        opt.Password = configuration.GetSection("RabbitMQ:Password").Value;
                        opt.Port = Convert.ToInt32(configuration.GetSection("RabbitMQ:Port").Value);
                        opt.CreateConnection();
                    };
                });
                /*x.UseDashboard(opt => opt.PathMatch = "/cap-dashboard");
                x.FailedRetryCount = 5;
                x.UseDispatchingPerGroup = true;
                x.FailedThresholdCallback = failed =>
                {
                    //var logger = failed.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    //logger.LogError($@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times, 
                    //    requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
                };
                x.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);*/
            });
            #endregion


            #region Serilog MongoDb Settings
            //var logger = CreateLogger(configuration);
            //services.AddLogging(x => x.AddSerilog(logger));
            #endregion
        }



        private static void AddRepositories(this IServiceCollection services)
        {
            services
                .AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork))
                .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<ApplicationDbContext>();
        }

        public static IServiceCollection AddMongoDbInitializer<T>(this IServiceCollection services, string connectionString, string databaseName) where T : class
        {
            services.AddHostedService(provider =>
            {
                var mongoUrl = new MongoUrl(connectionString);
                var mongoClient = new MongoClient(mongoUrl);
                var database = mongoClient.GetDatabase(databaseName);
                var collection = database.GetCollection<T>(typeof(T).Name);

                return new MongoDbInitializer<T>(mongoUrl, database, collection);
            });
            return services;
        }

        private static Serilog.ILogger CreateLogger(IConfiguration configuration)
        {
            var serilogSeqUrl = configuration.GetSection("SerilogSeqUrl").Value;
            var serilogConnectionString = configuration.GetConnectionString("SeriLogConnection");
            var minimumLevel = configuration.GetValue<LogEventLevel>("Serilog:MinimumLevel:Default");

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .WriteTo.Seq(serilogSeqUrl)
                .WriteTo.MongoDB(configuration["Databases:SerilogDatabaseName"], "loglama", minimumLevel);
            /*.WriteTo.MSSqlServer(
                connectionString: serilogConnectionString,
                sinkOptions: new MSSqlServerSinkOptions { AutoCreateSqlDatabase = true, AutoCreateSqlTable = true, TableName = "LogEvents" });*/

            return loggerConfiguration.CreateLogger();
        }


    }
}
