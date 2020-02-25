using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trivadis.PlateDetection.Model;

namespace Trivadis.PlateDetection
{
    public class ApplicationDatabaseContext : DbContext
    {
        private readonly ILoggerFactory loggerFactory;
        public ApplicationDatabaseContext(DbContextOptions options, ILoggerFactory loggerFactory)
            : base(options)
        {

            this.loggerFactory = loggerFactory;
        }
        public ApplicationDatabaseContext(ILoggerFactory loggerFactory)
            : base()
        {
            this.loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLoggerFactory(loggerFactory);
            optionsBuilder.UseSnakeCaseNamingConvention();
            optionsBuilder.UseLowerCaseNamingConvention();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is ITrackable trackable)
                {
                    var now = System.DateTime.UtcNow;
                    var user = Environment.UserName;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.LastUpdatedAt = now;
                            trackable.LastUpdatedBy = user;
                            break;

                        case EntityState.Added:
                            trackable.CreatedAt = now;
                            trackable.CreatedBy = user;
                            trackable.LastUpdatedAt = now;
                            trackable.LastUpdatedBy = user;
                            break;
                    }
                }
            }
        }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<DetectionResult> Results { get;set; }

        public DbSet<DetectedPlate> Plates { get; set; }
    }   
}
