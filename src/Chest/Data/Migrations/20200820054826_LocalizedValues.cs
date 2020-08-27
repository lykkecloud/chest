using Microsoft.EntityFrameworkCore.Migrations;

namespace Chest.Migrations
{
    public partial class LocalizedValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_localization",
                schema: "chest",
                columns: table => new
                {
                    Locale = table.Column<string>(maxLength: 10, nullable: false),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_localization", x => new { x.Key, x.Locale });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_localization",
                schema: "chest");
        }
    }
}
