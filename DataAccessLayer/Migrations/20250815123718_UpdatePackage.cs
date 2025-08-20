using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "can_post_blog",
                table: "package",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_reuse_previous_escort_paths",
                table: "package",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "can_view_incident_detail",
                table: "package",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "monthly_virtual_escort_limit",
                table: "package",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "approved_by",
                table: "blog",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "configuration",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category = table.Column<string>(type: "text", nullable: false),
                    key = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_configuration", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blog_approved_by",
                table: "blog",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_configuration_category_key",
                table: "configuration",
                columns: new[] { "category", "key" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_blog_account_approved_by",
                table: "blog",
                column: "approved_by",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blog_account_approved_by",
                table: "blog");

            migrationBuilder.DropTable(
                name: "configuration");

            migrationBuilder.DropIndex(
                name: "IX_blog_approved_by",
                table: "blog");

            migrationBuilder.DropColumn(
                name: "can_post_blog",
                table: "package");

            migrationBuilder.DropColumn(
                name: "can_reuse_previous_escort_paths",
                table: "package");

            migrationBuilder.DropColumn(
                name: "can_view_incident_detail",
                table: "package");

            migrationBuilder.DropColumn(
                name: "monthly_virtual_escort_limit",
                table: "package");

            migrationBuilder.DropColumn(
                name: "approved_by",
                table: "blog");
        }
    }
}
