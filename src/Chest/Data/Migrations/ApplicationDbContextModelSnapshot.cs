﻿// <auto-generated />
using Chest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Chest.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011");

            modelBuilder.Entity("Chest.Data.KeyValueData", b =>
                {
                    b.Property<string>("Category")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("category")
                        .HasDefaultValue("metadata")
                        .HasMaxLength(100);

                    b.Property<string>("Collection")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("collection")
                        .HasDefaultValue("metadata")
                        .HasMaxLength(100);

                    b.Property<string>("Key")
                        .HasColumnName("key")
                        .HasMaxLength(100);

                    b.Property<string>("DisplayCategory")
                        .IsRequired()
                        .HasColumnName("display_category")
                        .HasMaxLength(100);

                    b.Property<string>("DisplayCollection")
                        .IsRequired()
                        .HasColumnName("display_collection")
                        .HasMaxLength(100);

                    b.Property<string>("DisplayKey")
                        .IsRequired()
                        .HasColumnName("display_key")
                        .HasMaxLength(100);

                    b.Property<string>("MetaData")
                        .IsRequired()
                        .HasColumnName("meta_data")
                        .HasMaxLength(4096);

                    b.HasKey("Category", "Collection", "Key");

                    b.ToTable("key_value_data");
                });
#pragma warning restore 612, 618
        }
    }
}
