using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chest.Migrations
{
    public partial class AddAuditTrail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_audit",
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
                    table.PrimaryKey("PK_tb_audit", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_audit",
                schema: "chest");
        }
    }
}
