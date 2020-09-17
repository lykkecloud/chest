using Chest.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chest.Data.EntityConfigurations
{
    public class LocalizedValueEntityConfiguration : IEntityTypeConfiguration<LocalizedValue>
    {
        public void Configure(EntityTypeBuilder<LocalizedValue> builder)
        {
            builder.HasKey(lv => new {lv.Key, lv.Locale});

            builder.Property(x => x.Locale).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Key).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Value).IsRequired().HasMaxLength(4096);
        }
    }
}