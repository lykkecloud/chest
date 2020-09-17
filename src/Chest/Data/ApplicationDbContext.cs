// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Chest.Data.Entities;

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
