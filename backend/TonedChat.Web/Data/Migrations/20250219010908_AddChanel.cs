using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TonedChat.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "channel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    create_date = table.Column<string>(type: "TEXT", nullable: false),
                    update_date = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channel", x => x.id);
                });
            
            migrationBuilder.AddColumn<Guid>(
                name: "channel_id",
                table: "chat_message",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_chat_message_channel_id",
                table: "chat_message",
                column: "channel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_chat_message_channel_channel_id",
                table: "chat_message",
                column: "channel_id",
                principalTable: "channel",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_chat_message_channel_channel_id",
                table: "chat_message");

            migrationBuilder.DropTable(
                name: "channel");

            migrationBuilder.DropIndex(
                name: "ix_chat_message_channel_id",
                table: "chat_message");

            migrationBuilder.DropColumn(
                name: "channel_id",
                table: "chat_message");
        }
    }
}
