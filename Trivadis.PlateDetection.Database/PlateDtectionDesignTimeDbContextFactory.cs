using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Trivadis.PlateDetection.Database
{
    public class PlateDtectionDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDatabaseContext>
    {
        public ApplicationDatabaseContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDatabaseContext>();
            builder.UseNpgsql("Host=127.0.0.1;Database=Demo;Username=demo;Password=demo");
            return new ApplicationDatabaseContext(builder.Options, null);
        }
    }
}
