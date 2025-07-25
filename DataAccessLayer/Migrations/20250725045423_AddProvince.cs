using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddProvince : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "province_id",
                table: "commune",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "province",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_province", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_commune_province_id",
                table: "commune",
                column: "province_id");

            migrationBuilder.AddForeignKey(
                name: "FK_commune_province_province_id",
                table: "commune",
                column: "province_id",
                principalTable: "province",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_commune_province_province_id",
                table: "commune");

            migrationBuilder.DropTable(
                name: "province");

            migrationBuilder.DropIndex(
                name: "IX_commune_province_id",
                table: "commune");

            migrationBuilder.DropColumn(
                name: "province_id",
                table: "commune");
        }
    }
}
