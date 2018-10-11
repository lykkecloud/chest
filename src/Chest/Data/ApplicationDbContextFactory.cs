// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Data
{
    using System.IO;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    // NOTE (Cameron): This is required for Entity Framework migrations as the signature of BuildWebHost has been modified.
    // LINK (Cameron): https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentSecrets(args).Build();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Chest"))
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
