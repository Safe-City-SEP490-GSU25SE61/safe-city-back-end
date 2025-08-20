using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEscortGroupSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_escort_group_join_request_group_id_account_id",
                table: "escort_group_join_request");

            migrationBuilder.AddColumn<bool>(
                name: "auto_approve",
                table: "escort_journey_group",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "receive_request",
                table: "escort_journey_group",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_escort_group_join_request_group_id",
                table: "escort_group_join_request",
                column: "group_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_escort_group_join_request_group_id",
                table: "escort_group_join_request");

            migrationBuilder.DropColumn(
                name: "auto_approve",
                table: "escort_journey_group");

            migrationBuilder.DropColumn(
                name: "receive_request",
                table: "escort_journey_group");

            migrationBuilder.CreateIndex(
                name: "IX_escort_group_join_request_group_id_account_id",
                table: "escort_group_join_request",
                columns: new[] { "group_id", "account_id" },
                unique: true);
        }
    }
}
