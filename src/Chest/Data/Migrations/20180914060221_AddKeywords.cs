namespace Chest.Migrations
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddKeywords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "keywords",
                table: "key_value_data",
                maxLength: 1024,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "keywords",
                table: "key_value_data");
        }
    }
}
