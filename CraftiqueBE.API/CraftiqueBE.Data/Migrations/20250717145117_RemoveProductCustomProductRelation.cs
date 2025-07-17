using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftiqueBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductCustomProductRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomProducts_Products_ProductID",
                table: "CustomProducts");

            migrationBuilder.DropIndex(
                name: "IX_CustomProducts_ProductID",
                table: "CustomProducts");

            migrationBuilder.AddColumn<int>(
                name: "ProductID",
                table: "CustomProductFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductID",
                table: "CustomProductFiles");

            migrationBuilder.CreateIndex(
                name: "IX_CustomProducts_ProductID",
                table: "CustomProducts",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomProducts_Products_ProductID",
                table: "CustomProducts",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
