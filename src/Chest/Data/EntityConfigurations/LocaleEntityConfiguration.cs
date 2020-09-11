using Chest.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chest.Data.EntityConfigurations
{
    public class LocaleEntityConfiguration : IEntityTypeConfiguration<Locale>
    {
        public void Configure(EntityTypeBuilder<Locale> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired().HasMaxLength(100);
        }
    }
}