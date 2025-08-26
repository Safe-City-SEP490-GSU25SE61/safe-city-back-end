using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAllDB826 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
    name: "FK_escort_journey_watcher_account_watcher_id",
    table: "escort_journey_watcher");

            migrationBuilder.DropColumn(
                name: "watcher_id",
                table: "escort_journey_watcher");

            migrationBuilder.AddColumn<int>(
                name: "watcher_id",
                table: "escort_journey_watcher",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_escort_journey_watcher_escort_journey_group_member_watcher_id",
                table: "escort_journey_watcher",
                column: "watcher_id",
                principalTable: "escort_journey_group_member",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_escort_journey_watcher_escort_journey_group_member_watcher_~",
                table: "escort_journey_watcher");

            migrationBuilder.AlterColumn<Guid>(
                name: "watcher_id",
                table: "escort_journey_watcher",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_escort_journey_watcher_account_watcher_id",
                table: "escort_journey_watcher",
                column: "watcher_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
