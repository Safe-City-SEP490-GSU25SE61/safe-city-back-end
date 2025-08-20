using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEscortGroup817 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expected_time",
                table: "escort_journey");

            migrationBuilder.AlterColumn<double>(
                name: "start_longitude",
                table: "escort_journey",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "start_latitude",
                table: "escort_journey",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "end_longitude",
                table: "escort_journey",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "end_latitude",
                table: "escort_journey",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "distance_in_meters",
                table: "escort_journey",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "duration_in_seconds",
                table: "escort_journey",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "route_json",
                table: "escort_journey",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "vehicle",
                table: "escort_journey",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "distance_in_meters",
                table: "escort_journey");

            migrationBuilder.DropColumn(
                name: "duration_in_seconds",
                table: "escort_journey");

            migrationBuilder.DropColumn(
                name: "route_json",
                table: "escort_journey");

            migrationBuilder.DropColumn(
                name: "vehicle",
                table: "escort_journey");

            migrationBuilder.AlterColumn<decimal>(
                name: "start_longitude",
                table: "escort_journey",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "start_latitude",
                table: "escort_journey",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "end_longitude",
                table: "escort_journey",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "end_latitude",
                table: "escort_journey",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "expected_time",
                table: "escort_journey",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
