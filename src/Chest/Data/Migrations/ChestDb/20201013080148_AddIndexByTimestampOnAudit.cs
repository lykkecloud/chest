using Microsoft.EntityFrameworkCore.Migrations;

namespace Chest.Data.Migrations.ChestDb
{
    public partial class AddIndexByTimestampOnAudit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AuditTrail_Timestamp",
                schema: "chest",
                table: "AuditTrail",
                column: "Timestamp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuditTrail_Timestamp",
                schema: "chest",
                table: "AuditTrail");
        }
    }
}
