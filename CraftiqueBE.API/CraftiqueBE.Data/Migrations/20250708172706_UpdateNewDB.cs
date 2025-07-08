using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftiqueBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNewDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_CustomProducts_CustomProductID",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "CustomProductID",
                table: "OrderDetails",
                newName: "CustomProductFileID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_CustomProductID",
                table: "OrderDetails",
                newName: "IX_OrderDetails_CustomProductFileID");

            migrationBuilder.AlterColumn<int>(
                name: "ProductItemID",
                table: "OrderDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "CustomProductFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_CustomProductFiles_CustomProductFileID",
                table: "OrderDetails",
                column: "CustomProductFileID",
                principalTable: "CustomProductFiles",
                principalColumn: "CustomProductFileID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_CustomProductFiles_CustomProductFileID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CustomProductFiles");

            migrationBuilder.RenameColumn(
                name: "CustomProductFileID",
                table: "OrderDetails",
                newName: "CustomProductID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_CustomProductFileID",
                table: "OrderDetails",
                newName: "IX_OrderDetails_CustomProductID");

            migrationBuilder.AlterColumn<int>(
                name: "ProductItemID",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_CustomProducts_CustomProductID",
                table: "OrderDetails",
                column: "CustomProductID",
                principalTable: "CustomProducts",
                principalColumn: "CustomProductID");
        }
    }
}
