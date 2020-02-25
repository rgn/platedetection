using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Trivadis.PlateDetection.Broker;
using Trivadis.PlateDetection.Database;
using Trivadis.PlateDetection.Model;

namespace Trivadis.PlateDetection
{
    public static class PlateDetectionWorkerCollectionExtensions
    {
        public static IConfigurationBuilder AddAppConfiguration(this IConfigurationBuilder builder)
        {
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            // todo: add environment-specific appsettings loading
            return builder;
        }

        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
            {
                {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
                {"level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                {"raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp) },
                {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                {"properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
                {"props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
                {"machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") }
            };

            Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Verbose()
                                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                .WriteTo.ColoredConsole()
                                .WriteTo.PostgreSQL(configuration.GetConnectionString("PlateDetectionDB"), "logs", columnWriters, needAutoCreateTable: true)
                                .CreateLogger();

            services.AddSingleton<Microsoft.Extensions.Logging.ILoggerFactory>(new Serilog.Extensions.Logging.SerilogLoggerFactory(Log.Logger));
            services.AddSingleton<Serilog.ILogger>(Log.Logger);

            if (configuration["PD_RUNMODE"] == "Production")
            {
                services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
            }
            else
            {
                services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Trace);
            }

            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddSingleton(configuration);
            services.AddTransient<IDbInitializer<ApplicationDatabaseContext>, DbInitializer>();
            services.AddOptions();
            services.AddLogging(configuration);
            
            services.Configure<PlateDetectionServiceOptions>(configuration.GetSection("PlateDetectionService"));
            services.AddTransient<PlateDetectionService>();

            services.AddDbContext<ApplicationDatabaseContext>(options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString("PlateDetectionDB"));
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                    options.EnableServiceProviderCaching();
                }
                , ServiceLifetime.Transient, ServiceLifetime.Transient
            );

            services.Configure<BrokerProducerServiceOptions>(configuration.GetSection("BrokerProducer"));
            services.AddTransient<KeylessBrokerProducerService<Job>>();

            return services;
        }
        
        public static IServiceProvider FlushLogger(this IServiceProvider service)
        {
            if (Log.Logger is IDisposable log)
            {
                log.Dispose();
            }

            return service;
        }
    }
}
