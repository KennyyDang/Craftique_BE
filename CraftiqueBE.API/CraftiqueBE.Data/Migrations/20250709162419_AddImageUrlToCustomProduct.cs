using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftiqueBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToCustomProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "CustomProducts",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "CustomProducts");
        }
    }
}
