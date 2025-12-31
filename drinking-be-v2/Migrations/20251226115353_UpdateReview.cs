using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace drinking_be.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cart_user_id_status",
                table: "cart");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "review",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true,
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddColumn<long>(
                name: "OrderId",
                table: "review",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_review_OrderId",
                table: "review",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_cart_user_id_store_id_status",
                table: "cart",
                columns: new[] { "user_id", "store_id", "status" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_review_order_OrderId",
                table: "review",
                column: "OrderId",
                principalTable: "order",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_review_order_OrderId",
                table: "review");

            migrationBuilder.DropIndex(
                name: "IX_review_OrderId",
                table: "review");

            migrationBuilder.DropIndex(
                name: "IX_cart_user_id_store_id_status",
                table: "cart");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "review");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "review",
                type: "timestamp without time zone",
                nullable: true,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "NOW()");

            migrationBuilder.CreateIndex(
                name: "IX_cart_user_id_status",
                table: "cart",
                columns: new[] { "user_id", "status" });
        }
    }
}
