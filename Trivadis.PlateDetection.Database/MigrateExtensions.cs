using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Trivadis.PlateDetection.Database
{
    public static class MigrateExtensions
    {
        public static IServiceProvider Migrate<T>(this IServiceProvider serviceProvider) where T : DbContext
        {
            serviceProvider.GetService<T>().Database.Migrate();

            return serviceProvider;
        }
    }
}
