using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TonedChat.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var createGeneralChannelSql = @"INSERT INTO channel(id, name, create_date, update_date) values ('70227176-b079-4e8d-bd30-c48a487733da', 'general', strftime('%Y-%m-%d %H:%M:%f', 'now'), strftime('%Y-%m-%d %H:%M:%f', 'now'))";
            migrationBuilder.Sql(createGeneralChannelSql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
