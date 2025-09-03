using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogModeration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "district_id",
                table: "blog",
                newName: "commune_id");

            migrationBuilder.RenameIndex(
                name: "IX_blog_district_id",
                table: "blog",
                newName: "IX_blog_commune_id");

            migrationBuilder.CreateTable(
                name: "blog_moderation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    blog_id = table.Column<int>(type: "integer", nullable: false),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false),
                    politeness = table.Column<bool>(type: "boolean", nullable: false),
                    no_anti_state = table.Column<bool>(type: "boolean", nullable: false),
                    positive_meaning = table.Column<bool>(type: "boolean", nullable: false),
                    type_requirement = table.Column<bool>(type: "boolean", nullable: false),
                    reasoning = table.Column<string>(type: "text", nullable: false),
                    violations = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_moderation", x => x.id);
                    table.ForeignKey(
                        name: "FK_blog_moderation_blog_blog_id",
                        column: x => x.blog_id,
                        principalTable: "blog",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blog_moderation_blog_id",
                table: "blog_moderation",
                column: "blog_id");

            migrationBuilder.AddForeignKey(
                name: "FK_blog_commune_commune_id",
                table: "blog",
                column: "commune_id",
                principalTable: "commune",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_blog_commune_commune_id",
                table: "blog");

            migrationBuilder.DropTable(
                name: "blog_moderation");

            migrationBuilder.RenameColumn(
                name: "commune_id",
                table: "blog",
                newName: "district_id");

            migrationBuilder.RenameIndex(
                name: "IX_blog_commune_id",
                table: "blog",
                newName: "IX_blog_district_id");

            migrationBuilder.CreateTable(
                name: "District",
                columns: table => new
                {
                    TempId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.UniqueConstraint("AK_District_TempId", x => x.TempId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_blog_District_district_id",
                table: "blog",
                column: "district_id",
                principalTable: "District",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
