using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace drinking_be.Migrations
{
    /// <inheritdoc />
    public partial class AddBrandIdToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "product",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_product_BrandId",
                table: "product",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_product_brand_BrandId",
                table: "product",
                column: "BrandId",
                principalTable: "brand",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_brand_BrandId",
                table: "product");

            migrationBuilder.DropIndex(
                name: "IX_product_BrandId",
                table: "product");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "product");
        }
    }
}
