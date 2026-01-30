using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace drinking_be.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_brand_BrandId",
                table: "product");

            migrationBuilder.DropForeignKey(
                name: "FK_product_category_CategoryId",
                table: "product");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "product",
                newName: "category_id");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "product",
                newName: "brand_id");

            migrationBuilder.RenameIndex(
                name: "IX_product_CategoryId",
                table: "product",
                newName: "IX_product_category_id");

            migrationBuilder.RenameIndex(
                name: "IX_product_BrandId",
                table: "product",
                newName: "IX_product_brand_id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_brand_brand_id",
                table: "product",
                column: "brand_id",
                principalTable: "brand",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_product_category_category_id",
                table: "product",
                column: "category_id",
                principalTable: "category",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_brand_brand_id",
                table: "product");

            migrationBuilder.DropForeignKey(
                name: "FK_product_category_category_id",
                table: "product");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "product",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "brand_id",
                table: "product",
                newName: "BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_product_category_id",
                table: "product",
                newName: "IX_product_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_product_brand_id",
                table: "product",
                newName: "IX_product_BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_product_brand_BrandId",
                table: "product",
                column: "BrandId",
                principalTable: "brand",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_product_category_CategoryId",
                table: "product",
                column: "CategoryId",
                principalTable: "category",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
