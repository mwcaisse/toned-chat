using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TonedChat.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chat_message",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    user_name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    content = table.Column<string>(type: "TEXT", nullable: false),
                    date = table.Column<string>(type: "TEXT", nullable: false),
                    create_date = table.Column<string>(type: "TEXT", nullable: false),
                    update_date = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_message", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_message");
        }
    }
}
