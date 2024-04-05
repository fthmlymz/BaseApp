using Application.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Persistence.Context;
using Persistence.Repositories;
using Serilog;
using Serilog.Events;
using System.Text.Encodings.Web;
using System.Text.Unicode;

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
            #region Serilog MongoDb Settings
            var logger = CreateLogger(configuration);
            services.AddLogging(x => x.AddSerilog(logger));
            #endregion


            #region EventBus - DotnetCore Cap MongoDB for RabbitMq
            services.AddCap(x =>
            {
                x.UseMongoDB(opt =>
                {
                    var mongoUrl = new MongoUrl(configuration.GetConnectionString("DotnetCoreCapConnection"));
                    opt.DatabaseConnection = mongoUrl.Url;
                    opt.DatabaseName = mongoUrl.DatabaseName;
                });
                x.UseRabbitMQ(options =>
                {
                    options.ExchangeName = "BaseApiExchangeName";
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
                x.UseDashboard(opt => opt.PathMatch = "/cap-dashboard");
                x.FailedRetryCount = 5;
                x.UseDispatchingPerGroup = true;
                x.FailedThresholdCallback = failed =>
                {
                    //var logger = failed.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    //logger.LogError($@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times, 
                    //    requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
                };
                x.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            });
            #endregion
        }


        private static void AddRepositories(this IServiceCollection services)
        {
            services
                .AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork))
                .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<ApplicationDbContext>();
        }

        private static ILogger CreateLogger(IConfiguration configuration)
        {
            var serilogSeqUrl = configuration["SerilogSeqUrl"];
            var seriLogConnection = configuration.GetConnectionString("SeriLogConnection");
            var minimumLevel = configuration.GetValue<LogEventLevel>("Serilog:MinimumLevel:Default");


            var loggerConfiguration = new LoggerConfiguration().MinimumLevel.Is(minimumLevel);

            if (!string.IsNullOrEmpty(serilogSeqUrl))
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Seq(serilogSeqUrl, minimumLevel);
            }
            if (!string.IsNullOrEmpty(seriLogConnection))
            {
                loggerConfiguration = loggerConfiguration.WriteTo.MongoDB(seriLogConnection, "Logs", minimumLevel);
            }

            return loggerConfiguration.CreateLogger();
        }
    }
}
