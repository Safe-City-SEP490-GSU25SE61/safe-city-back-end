using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_blog_created_at",
                table: "blog",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_blog_is_visible_is_approved",
                table: "blog",
                columns: new[] { "is_visible", "is_approved" });

            migrationBuilder.CreateIndex(
                name: "IX_blog_type",
                table: "blog",
                column: "type");

            // GIN index on title with to_tsvector (full-text search)
            migrationBuilder.Sql(
                "CREATE INDEX IX_blog_title_gin ON blog USING gin (to_tsvector('simple', title));");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_blog_created_at",
                table: "blog");

            migrationBuilder.DropIndex(
                name: "IX_blog_is_visible_is_approved",
                table: "blog");

            migrationBuilder.DropIndex(
                name: "IX_blog_type",
                table: "blog");

            migrationBuilder.Sql(
                "DROP INDEX IF EXISTS IX_blog_title_gin;");
        }

    }
}
