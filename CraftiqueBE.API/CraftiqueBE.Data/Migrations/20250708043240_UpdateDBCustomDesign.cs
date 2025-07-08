using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftiqueBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBCustomDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductCustomizationID",
                table: "OrderDetails",
                newName: "CustomProductID");

            migrationBuilder.CreateTable(
                name: "CustomProducts",
                columns: table => new
                {
                    CustomProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomProducts", x => x.CustomProductID);
                    table.ForeignKey(
                        name: "FK_CustomProducts_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomProductFiles",
                columns: table => new
                {
                    CustomProductFileID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomProductID = table.Column<int>(type: "int", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CustomText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomProductFiles", x => x.CustomProductFileID);
                    table.ForeignKey(
                        name: "FK_CustomProductFiles_CustomProducts_CustomProductID",
                        column: x => x.CustomProductID,
                        principalTable: "CustomProducts",
                        principalColumn: "CustomProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_CustomProductID",
                table: "OrderDetails",
                column: "CustomProductID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomProductFiles_CustomProductID",
                table: "CustomProductFiles",
                column: "CustomProductID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomProducts_ProductID",
                table: "CustomProducts",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_CustomProducts_CustomProductID",
                table: "OrderDetails",
                column: "CustomProductID",
                principalTable: "CustomProducts",
                principalColumn: "CustomProductID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_CustomProducts_CustomProductID",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "CustomProductFiles");

            migrationBuilder.DropTable(
                name: "CustomProducts");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_CustomProductID",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "CustomProductID",
                table: "OrderDetails",
                newName: "ProductCustomizationID");
        }
    }
}
