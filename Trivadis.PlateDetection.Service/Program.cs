using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Topshelf;
using Topshelf.MicrosoftDependencyInjection;
using Trivadis.PlateDetection.Database;

namespace Trivadis.PlateDetection
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddAppConfiguration()
                .Build();

            IServiceProvider serviceProvider = new ServiceCollection()
                .AddAppServices(configuration)
                .BuildServiceProvider();

            // Initialize and fill database
            serviceProvider.Migrate<ApplicationDatabaseContext>().Seed<ApplicationDatabaseContext>();
            
            var rc = HostFactory.Run(x =>
            {
                x.UseMicrosoftDependencyInjection(serviceProvider);
                //x.UseLoggingExtensions(y);
                x.UseSerilog();
                x.Service<PlateDetectionService>(s =>
                {
                    s.ConstructUsingMicrosoftDependencyInjection<PlateDetectionService>();
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();
                //x.UseSerilog(serviceProvider.GetService<Serilog.ILogger>());
                x.SetDescription("PlateDetection service.");
                x.SetDisplayName("Trivadis.PlateDetection.Service");
                x.SetServiceName("Trivadis.PlateDetection.Service");
            });

            // flush logger before the app closes
            serviceProvider.FlushLogger();

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
