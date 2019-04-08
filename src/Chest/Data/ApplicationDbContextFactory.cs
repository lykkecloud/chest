// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

namespace Chest.Data
{
    using System.Collections.Generic;
    using System.IO;
    using Lykke.Snow.Common.Startup;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    // NOTE (Cameron): This is required for Entity Framework migrations as the signature of BuildWebHost has been modified.
    // LINK (Cameron): https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private static readonly List<(string, string, string)> EnvironmentSecretConfig = new List<(string, string, string)>
        {
            /* secrets.json Key             // Environment Variable         // default value (optional) */
            ("ConnectionStrings:Chest",     "CHEST_CONNECTIONSTRING",       null)
        };

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentSecrets<Startup>(EnvironmentSecretConfig)
                .Build();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Chest"))
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
