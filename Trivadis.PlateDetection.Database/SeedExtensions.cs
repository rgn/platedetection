using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Trivadis.PlateDetection.Database
{
    public static class SeedExtensions
    {
        public static IServiceProvider Seed<T>(this IServiceProvider serviceProvider) where T : DbContext
        {
            serviceProvider.GetService<IDbInitializer<T>>().EnsureSeededAsync();
            return serviceProvider;
        }
    }
}
