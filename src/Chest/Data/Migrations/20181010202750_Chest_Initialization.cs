// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Chest.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Chest_Initialization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "chest");

            migrationBuilder.CreateTable(
                name: "tb_keyValueData",
                schema: "chest",
                columns: table => new
                {
                    Category = table.Column<string>(maxLength: 100, nullable: false),
                    Collection = table.Column<string>(maxLength: 100, nullable: false),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    DisplayCategory = table.Column<string>(maxLength: 100, nullable: false),
                    DisplayCollection = table.Column<string>(maxLength: 100, nullable: false),
                    DisplayKey = table.Column<string>(maxLength: 100, nullable: false),
                    MetaData = table.Column<string>(maxLength: 4096, nullable: false),
                    Keywords = table.Column<string>(maxLength: 1024, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_keyValueData", x => new { x.Category, x.Collection, x.Key });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_keyValueData",
                schema: "chest");
        }
    }
}
