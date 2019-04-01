// (c) Lykke Corporation 2019 - All rights reserved. No copying, adaptation, decompiling, distribution or any other form of use permitted.

namespace Chest.Data
{
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        internal DbSet<KeyValueData> KeyValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // creating composite key as this is not possible with data annotations attribute
            modelBuilder
                .Entity<KeyValueData>()
                .HasKey(k => new { k.Category, k.Collection, k.Key });

            base.OnModelCreating(modelBuilder);
        }
    }
}
