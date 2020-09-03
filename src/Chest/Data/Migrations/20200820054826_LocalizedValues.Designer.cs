﻿// <auto-generated />
using Chest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chest.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200820054826_LocalizedValues")]
    partial class LocalizedValues
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Chest.Data.Entities.KeyValueData", b =>
                {
                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Collection")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("DisplayCategory")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("DisplayCollection")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("DisplayKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Keywords")
                        .HasColumnType("nvarchar(1024)")
                        .HasMaxLength(1024);

                    b.Property<string>("MetaData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasMaxLength(4096);

                    b.HasKey("Category", "Collection", "Key");

                    b.ToTable("tb_keyValueData","chest");
                });

            modelBuilder.Entity("Chest.Data.Entities.LocalizedValue", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("Locale")
                        .HasColumnType("nvarchar(10)")
                        .HasMaxLength(10);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasMaxLength(4096);

                    b.HasKey("Key", "Locale");

                    b.ToTable("tb_localization","chest");
                });
#pragma warning restore 612, 618
        }
    }
}