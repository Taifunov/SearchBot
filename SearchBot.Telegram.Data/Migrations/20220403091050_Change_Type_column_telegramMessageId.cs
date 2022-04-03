using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchBot.Telegram.Data.Migrations
{
    public partial class Change_Type_column_telegramMessageId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TelegramMessageId",
                table: "Messages",
                type: "integer",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "TelegramMessageId",
                table: "Messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
