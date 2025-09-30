using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddAgora : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "call_channel_name",
                table: "sos_alert",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "call_duration",
                table: "sos_alert",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "call_type",
                table: "sos_alert",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "sos_alert",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ended_at",
                table: "sos_alert",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "sos_alert",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "agora_uid",
                table: "escort_journey_watcher",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "call_session_name",
                table: "escort_journey_watcher",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "call_status",
                table: "escort_journey_watcher",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "expire_at",
                table: "escort_journey_watcher",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "issued_at",
                table: "escort_journey_watcher",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "join_time",
                table: "escort_journey_watcher",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_refreshed_at",
                table: "escort_journey_watcher",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "leave_time",
                table: "escort_journey_watcher",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "escort_journey_watcher",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "token",
                table: "escort_journey_watcher",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "call_channel_name",
                table: "sos_alert");

            migrationBuilder.DropColumn(
                name: "call_duration",
                table: "sos_alert");

            migrationBuilder.DropColumn(
                name: "call_type",
                table: "sos_alert");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "sos_alert");

            migrationBuilder.DropColumn(
                name: "ended_at",
                table: "sos_alert");

            migrationBuilder.DropColumn(
                name: "status",
                table: "sos_alert");

            migrationBuilder.DropColumn(
                name: "agora_uid",
                table: "escort_journey_watcher");

            migrationBuilder.DropColumn(
                name: "call_session_name",
                table: "escort_journey_watcher");

            migrationBuilder.DropColumn(
                name: "call_status",
                table: "escort_journey_watcher");

            migrationBuilder.DropColumn(
                name: "expire_at",
                table: "escort_journey_watcher");

            migrationBuilder.DropColumn(
                name: "issued_at",
                table: "escort_journey_watcher");

            migrationBuilder.DropColumn(
                name: "join_time",
                table: "escort_journey_watcher");

            migrationBuilder.DropColumn(
                name: "last_refreshed_at",
                table: "escort_journey_watcher");

            migrationBuilder.DropColumn(
                name: "leave_time",
                table: "escort_journey_watcher");

            migrationBuilder.DropColumn(
                name: "role",
                table: "escort_journey_watcher");

            migrationBuilder.DropColumn(
                name: "token",
                table: "escort_journey_watcher");
        }
    }
}
