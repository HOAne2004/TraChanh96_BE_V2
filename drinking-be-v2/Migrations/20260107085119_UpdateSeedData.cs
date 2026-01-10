using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace drinking_be.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payment_flow",
                table: "order");

            migrationBuilder.RenameColumn(
                name: "imageUrl",
                table: "room",
                newName: "ImageUrl");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "room",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "room",
                newName: "imageUrl");

            migrationBuilder.AlterColumn<string>(
                name: "imageUrl",
                table: "room",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "payment_flow",
                table: "order",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
