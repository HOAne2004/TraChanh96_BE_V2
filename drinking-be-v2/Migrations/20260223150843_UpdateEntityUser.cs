using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace drinking_be.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "user",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEnd",
                table: "user",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "user");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "user");
        }
    }
}
