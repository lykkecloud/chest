using Microsoft.EntityFrameworkCore.Migrations;

namespace Chest.Migrations
{
    public partial class AddLocales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_locales",
                schema: "chest",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 100, nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_locales", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_locales",
                schema: "chest");
        }
    }
}
