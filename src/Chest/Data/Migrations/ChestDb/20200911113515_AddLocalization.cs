using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chest.Data.Migrations.ChestDb
{
    public partial class AddLocalization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "chest");

            migrationBuilder.CreateTable(
                name: "AuditTrail",
                schema: "chest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    CorrelationId = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    DataType = table.Column<string>(nullable: false),
                    DataReference = table.Column<string>(nullable: false),
                    DataDiff = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locales",
                schema: "chest",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalizedValues",
                schema: "chest",
                columns: table => new
                {
                    Locale = table.Column<string>(maxLength: 100, nullable: false),
                    Key = table.Column<string>(maxLength: 200, nullable: false),
                    Value = table.Column<string>(maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedValues", x => new { x.Key, x.Locale });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrail",
                schema: "chest");

            migrationBuilder.DropTable(
                name: "Locales",
                schema: "chest");

            migrationBuilder.DropTable(
                name: "LocalizedValues",
                schema: "chest");
        }
    }
}
