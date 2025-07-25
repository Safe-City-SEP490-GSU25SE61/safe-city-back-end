using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blog",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    district_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    pinned = table.Column<bool>(type: "boolean", nullable: false),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog", x => x.id);
                    table.ForeignKey(
                        name: "FK_blog_account_author_id",
                        column: x => x.author_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_blog_district_district_id",
                        column: x => x.district_id,
                        principalTable: "district",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "blog_like",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    blog_id = table.Column<int>(type: "integer", nullable: false),
                    liked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_like", x => x.id);
                    table.ForeignKey(
                        name: "FK_blog_like_account_user_id",
                        column: x => x.user_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_blog_like_blog_blog_id",
                        column: x => x.blog_id,
                        principalTable: "blog",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "blog_media",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    blog_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    media_slot = table.Column<int>(type: "integer", nullable: false),
                    file_data = table.Column<string>(type: "text", nullable: false),
                    thumbnail_data = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_media", x => x.id);
                    table.ForeignKey(
                        name: "FK_blog_media_blog_blog_id",
                        column: x => x.blog_id,
                        principalTable: "blog",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comment",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<int>(type: "integer", nullable: true),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_flagged = table.Column<bool>(type: "boolean", nullable: false),
                    is_visible = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment", x => x.id);
                    table.ForeignKey(
                        name: "FK_comment_account_user_id",
                        column: x => x.user_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comment_blog_post_id",
                        column: x => x.post_id,
                        principalTable: "blog",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blog_author_id",
                table: "blog",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_blog_district_id",
                table: "blog",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "IX_blog_like_blog_id",
                table: "blog_like",
                column: "blog_id");

            migrationBuilder.CreateIndex(
                name: "IX_blog_like_user_id_blog_id",
                table: "blog_like",
                columns: new[] { "user_id", "blog_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_blog_media_blog_id",
                table: "blog_media",
                column: "blog_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_post_id",
                table: "comment",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_user_id",
                table: "comment",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blog_like");

            migrationBuilder.DropTable(
                name: "blog_media");

            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "blog");
        }
    }
}
