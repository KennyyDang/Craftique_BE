using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftiqueBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CustomProducts");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CustomProductFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CustomProductFiles");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CustomProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
