using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBlogModeration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_blog_moderation_blog_id",
                table: "blog_moderation");

            migrationBuilder.CreateIndex(
                name: "IX_blog_moderation_blog_id",
                table: "blog_moderation",
                column: "blog_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_blog_moderation_blog_id",
                table: "blog_moderation");

            migrationBuilder.CreateIndex(
                name: "IX_blog_moderation_blog_id",
                table: "blog_moderation",
                column: "blog_id");
        }
    }
}
