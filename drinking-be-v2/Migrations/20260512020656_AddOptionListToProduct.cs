using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace drinking_be.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionListToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short[]>(
                name: "AllowedIceLevels",
                table: "product",
                type: "smallint[]",
                nullable: true);

            migrationBuilder.AddColumn<short[]>(
                name: "AllowedSugarLevels",
                table: "product",
                type: "smallint[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedIceLevels",
                table: "product");

            migrationBuilder.DropColumn(
                name: "AllowedSugarLevels",
                table: "product");
        }
    }
}
