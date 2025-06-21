using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountAndIdentityCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "verified_at",
                table: "citizen_identity_card");

            migrationBuilder.AddColumn<string>(
                name: "place_of_birth",
                table: "citizen_identity_card",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "code_expiry",
                table: "account",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "place_of_birth",
                table: "citizen_identity_card");

            migrationBuilder.DropColumn(
                name: "code_expiry",
                table: "account");

            migrationBuilder.AddColumn<DateTime>(
                name: "verified_at",
                table: "citizen_identity_card",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
