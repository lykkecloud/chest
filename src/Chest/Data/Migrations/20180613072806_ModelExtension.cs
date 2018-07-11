// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Migrations
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class ModelExtension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_key_value_data",
                table: "key_value_data");

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "key_value_data",
                maxLength: 100,
                nullable: false,
                defaultValue: "metadata");

            migrationBuilder.AddColumn<string>(
                name: "collection",
                table: "key_value_data",
                maxLength: 100,
                nullable: false,
                defaultValue: "metadata");

            migrationBuilder.AddColumn<string>(
                name: "display_category",
                table: "key_value_data",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "display_collection",
                table: "key_value_data",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_key_value_data",
                table: "key_value_data",
                columns: new[] { "category", "collection", "key" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_key_value_data",
                table: "key_value_data");

            migrationBuilder.DropColumn(
                name: "category",
                table: "key_value_data");

            migrationBuilder.DropColumn(
                name: "collection",
                table: "key_value_data");

            migrationBuilder.DropColumn(
                name: "display_category",
                table: "key_value_data");

            migrationBuilder.DropColumn(
                name: "display_collection",
                table: "key_value_data");

            migrationBuilder.AddPrimaryKey(
                name: "PK_key_value_data",
                table: "key_value_data",
                column: "key");
        }
    }
}
