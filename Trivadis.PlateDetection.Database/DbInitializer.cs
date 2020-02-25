using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Trivadis.PlateDetection.Model;

namespace Trivadis.PlateDetection.Database
{
    public class DbInitializer : IDbInitializer<ApplicationDatabaseContext>
    {
        private readonly ApplicationDatabaseContext db;

        public DbInitializer(ApplicationDatabaseContext db)
        {
            this.db = db;
        }

        public async Task EnsureSeededAsync()
        {
            if (db.Jobs.Any())
            {
                return;   // DB has been seeded
            }        

            // we do not net any initial data
            //await db.SaveChangesAsync();
        }
    }

    public interface IDbInitializer<T> where T : DbContext
    {
        Task EnsureSeededAsync();
    }
}
