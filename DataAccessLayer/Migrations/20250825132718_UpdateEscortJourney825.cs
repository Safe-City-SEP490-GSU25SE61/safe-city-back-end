using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEscortJourney825 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "current_user_location");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "subscription",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "subscription",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "payment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "payment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "member_id",
                table: "escort_journey",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_escort_journey_member_id",
                table: "escort_journey",
                column: "member_id");

            migrationBuilder.AddForeignKey(
                name: "FK_escort_journey_escort_journey_group_member_member_id",
                table: "escort_journey",
                column: "member_id",
                principalTable: "escort_journey_group_member",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_escort_journey_escort_journey_group_member_member_id",
                table: "escort_journey");

            migrationBuilder.DropIndex(
                name: "IX_escort_journey_member_id",
                table: "escort_journey");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "member_id",
                table: "escort_journey");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "subscription",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "subscription",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "current_user_location",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    escort_journey_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    battery_level = table.Column<int>(type: "integer", nullable: true),
                    heading = table.Column<decimal>(type: "numeric", nullable: true),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    speed = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_current_user_location", x => x.id);
                    table.ForeignKey(
                        name: "FK_current_user_location_account_user_id",
                        column: x => x.user_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_current_user_location_escort_journey_escort_journey_id",
                        column: x => x.escort_journey_id,
                        principalTable: "escort_journey",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_current_user_location_escort_journey_id",
                table: "current_user_location",
                column: "escort_journey_id");

            migrationBuilder.CreateIndex(
                name: "IX_current_user_location_user_id",
                table: "current_user_location",
                column: "user_id",
                unique: true);
        }
    }
}
