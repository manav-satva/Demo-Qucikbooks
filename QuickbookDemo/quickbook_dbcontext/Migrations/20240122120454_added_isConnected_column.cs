using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quickbook_dbcontext.Migrations
{
    /// <inheritdoc />
    public partial class added_isConnected_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConnected",
                table: "UserTokens",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConnected",
                table: "UserTokens");
        }
    }
}
