// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Chest.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chest.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder InitializeDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                // Set up Tables using Entity Framework (with Postgres).
                var applicationDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                applicationDbContext.Database.Migrate();
            }

            return app;
        }
    }
}
