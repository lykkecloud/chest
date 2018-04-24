// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Migrations
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class CreateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "key_value_data",
                columns: table => new
                {
                    key = table.Column<string>(maxLength: 100, nullable: false),
                    display_key = table.Column<string>(maxLength: 100, nullable: false),
                    metadata_data = table.Column<string>(maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_key_value_data", x => x.key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "key_value_data");
        }
    }
}
