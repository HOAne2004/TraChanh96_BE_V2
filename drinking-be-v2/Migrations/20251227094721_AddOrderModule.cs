using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace drinking_be.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_commentlike_comment_CommentId",
                table: "commentlike");

            migrationBuilder.DropForeignKey(
                name: "FK_commentlike_user_UserId",
                table: "commentlike");

            migrationBuilder.DropForeignKey(
                name: "FK_order_address_delivery_address_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_order_shop_table_table_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_order_user_voucher_user_voucher_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK__Order_ite__produ__2DE6D218",
                table: "order_item");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderPayment_PaymentMethodId",
                table: "order_payment");

            migrationBuilder.DropIndex(
                name: "IX_order_payment_payment_method_id",
                table: "order_payment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_commentlike",
                table: "commentlike");

            migrationBuilder.RenameTable(
                name: "commentlike",
                newName: "commentlikes");

            migrationBuilder.RenameIndex(
                name: "IX_commentlike_CommentId",
                table: "commentlikes",
                newName: "IX_commentlikes_CommentId");

            migrationBuilder.AddColumn<string>(
                name: "imageUrl",
                table: "room",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "order_payment",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true,
                oldDefaultValueSql: "NOW()");

            migrationBuilder.AddColumn<string>(
                name: "payment_method_name",
                table: "order_payment",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "type",
                table: "order_payment",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "ProductImage",
                table: "order_item",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "order_item",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SizeName",
                table: "order_item",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "voucher_code_used",
                table: "order",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_notes",
                table: "order",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "store_name",
                table: "order",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "pickup_code",
                table: "order",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "order_code",
                table: "order",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "order",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cancel_note",
                table: "order",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodName",
                table: "order",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "order",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhone",
                table: "order",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "order",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_commentlikes",
                table: "commentlikes",
                columns: new[] { "UserId", "CommentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_commentlikes_comment_CommentId",
                table: "commentlikes",
                column: "CommentId",
                principalTable: "comment",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_commentlikes_user_UserId",
                table: "commentlikes",
                column: "UserId",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_address_delivery_address_id",
                table: "order",
                column: "delivery_address_id",
                principalTable: "address",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_order_shop_table_table_id",
                table: "order",
                column: "table_id",
                principalTable: "shop_table",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_user_voucher_user_voucher_id",
                table: "order",
                column: "user_voucher_id",
                principalTable: "user_voucher",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_order_item_product_product_id",
                table: "order_item",
                column: "product_id",
                principalTable: "product",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_commentlikes_comment_CommentId",
                table: "commentlikes");

            migrationBuilder.DropForeignKey(
                name: "FK_commentlikes_user_UserId",
                table: "commentlikes");

            migrationBuilder.DropForeignKey(
                name: "FK_order_address_delivery_address_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_order_shop_table_table_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_order_user_voucher_user_voucher_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_order_item_product_product_id",
                table: "order_item");

            migrationBuilder.DropPrimaryKey(
                name: "PK_commentlikes",
                table: "commentlikes");

            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "room");

            migrationBuilder.DropColumn(
                name: "payment_method_name",
                table: "order_payment");

            migrationBuilder.DropColumn(
                name: "type",
                table: "order_payment");

            migrationBuilder.DropColumn(
                name: "ProductImage",
                table: "order_item");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "order_item");

            migrationBuilder.DropColumn(
                name: "SizeName",
                table: "order_item");

            migrationBuilder.DropColumn(
                name: "PaymentMethodName",
                table: "order");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "order");

            migrationBuilder.DropColumn(
                name: "RecipientPhone",
                table: "order");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "order");

            migrationBuilder.RenameTable(
                name: "commentlikes",
                newName: "commentlike");

            migrationBuilder.RenameIndex(
                name: "IX_commentlikes_CommentId",
                table: "commentlike",
                newName: "IX_commentlike_CommentId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "order_payment",
                type: "timestamp without time zone",
                nullable: true,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "voucher_code_used",
                table: "order",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "user_notes",
                table: "order",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "store_name",
                table: "order",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "pickup_code",
                table: "order",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "order_code",
                table: "order",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "order",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "cancel_note",
                table: "order",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_commentlike",
                table: "commentlike",
                columns: new[] { "UserId", "CommentId" });

            migrationBuilder.CreateIndex(
                name: "IX_order_payment_payment_method_id",
                table: "order_payment",
                column: "payment_method_id");

            migrationBuilder.AddForeignKey(
                name: "FK_commentlike_comment_CommentId",
                table: "commentlike",
                column: "CommentId",
                principalTable: "comment",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_commentlike_user_UserId",
                table: "commentlike",
                column: "UserId",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_address_delivery_address_id",
                table: "order",
                column: "delivery_address_id",
                principalTable: "address",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_shop_table_table_id",
                table: "order",
                column: "table_id",
                principalTable: "shop_table",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_order_user_voucher_user_voucher_id",
                table: "order",
                column: "user_voucher_id",
                principalTable: "user_voucher",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__Order_ite__produ__2DE6D218",
                table: "order_item",
                column: "product_id",
                principalTable: "product",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderPayment_PaymentMethodId",
                table: "order_payment",
                column: "payment_method_id",
                principalTable: "payment_method",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
