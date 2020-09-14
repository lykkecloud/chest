using System.Data.Common;
using Chest.Data.Entities;
using Lykke.Common.MsSql;
using Microsoft.EntityFrameworkCore;

namespace Chest.Data
{
    public class ChestDbContext : MsSqlContext
    {
        private const string Schema = "chest";

        public ChestDbContext() : base(Schema)
        {
        }
        
        public ChestDbContext(int commandTimeoutSeconds = 30) : base(Schema, commandTimeoutSeconds)
        {
        }

        public ChestDbContext(string connectionString, bool isTraceEnabled, int commandTimeoutSeconds = 30) : base(Schema, connectionString, isTraceEnabled, commandTimeoutSeconds)
        {
        }

        public ChestDbContext(DbContextOptions contextOptions) : base(Schema, contextOptions)
        {
        }

        public ChestDbContext(DbContextOptions options, bool isForMocks = false, int commandTimeoutSeconds = 30) : base(Schema, options, isForMocks, commandTimeoutSeconds)
        {
        }

        public ChestDbContext(DbConnection dbConnection, bool isForMocks = false, int commandTimeoutSeconds = 30) : base(Schema, dbConnection, isForMocks, commandTimeoutSeconds)
        {
        }
        
        internal DbSet<LocalizedValue> LocalizedValues { get; set; }

        internal DbSet<Locale> Locales { get; set; }
        
        internal DbSet<AuditEntity> AuditTrail { get; set; }


        protected override void OnLykkeModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}