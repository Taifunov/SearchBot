using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchBot.Telegram.Data.Migrations
{
    public partial class Renamed_prop_messages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastMessage",
                table: "Messages",
                newName: "Created");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Messages",
                newName: "LastMessage");
        }
    }
}
