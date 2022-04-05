using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchBot.Telegram.Data.Migrations
{
    public partial class Added_Banned_Col_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Banned",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Banned",
                table: "Users");
        }
    }
}
