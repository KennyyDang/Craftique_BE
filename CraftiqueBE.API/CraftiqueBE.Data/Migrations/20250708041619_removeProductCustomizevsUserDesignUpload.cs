using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CraftiqueBE.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeProductCustomizevsUserDesignUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_ProductCustomizations_ProductCustomizationID",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "ProductCustomizations");

            migrationBuilder.DropTable(
                name: "UserDesignUploads");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductCustomizationID",
                table: "OrderDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDesignUploads",
                columns: table => new
                {
                    UserDesignUploadID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDesignUploads", x => x.UserDesignUploadID);
                });

            migrationBuilder.CreateTable(
                name: "ProductCustomizations",
                columns: table => new
                {
                    ProductCustomizationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductItemID = table.Column<int>(type: "int", nullable: false),
                    UserDesignUploadID = table.Column<int>(type: "int", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CustomText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FontFamily = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PreviewImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCustomizations", x => x.ProductCustomizationID);
                    table.ForeignKey(
                        name: "FK_ProductCustomizations_ProductItems_ProductItemID",
                        column: x => x.ProductItemID,
                        principalTable: "ProductItems",
                        principalColumn: "ProductItemID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductCustomizations_UserDesignUploads_UserDesignUploadID",
                        column: x => x.UserDesignUploadID,
                        principalTable: "UserDesignUploads",
                        principalColumn: "UserDesignUploadID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductCustomizationID",
                table: "OrderDetails",
                column: "ProductCustomizationID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCustomizations_ProductItemID",
                table: "ProductCustomizations",
                column: "ProductItemID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCustomizations_UserDesignUploadID",
                table: "ProductCustomizations",
                column: "UserDesignUploadID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_ProductCustomizations_ProductCustomizationID",
                table: "OrderDetails",
                column: "ProductCustomizationID",
                principalTable: "ProductCustomizations",
                principalColumn: "ProductCustomizationID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
