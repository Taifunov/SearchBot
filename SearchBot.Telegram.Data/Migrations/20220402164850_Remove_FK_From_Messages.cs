using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchBot.Telegram.Data.Migrations
{
    public partial class Remove_FK_From_Messages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_TelegramUsers_TelegramUserId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_TelegramUserId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "TelegramUserId",
                table: "Messages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TelegramUserId",
                table: "Messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TelegramUserId",
                table: "Messages",
                column: "TelegramUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_TelegramUsers_TelegramUserId",
                table: "Messages",
                column: "TelegramUserId",
                principalTable: "TelegramUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
