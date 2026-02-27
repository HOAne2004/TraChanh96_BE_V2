using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace drinking_be.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationTokenExpiresAt",
                table: "user",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "user");

            migrationBuilder.DropColumn(
                name: "VerificationTokenExpiresAt",
                table: "user");
        }
    }
}
