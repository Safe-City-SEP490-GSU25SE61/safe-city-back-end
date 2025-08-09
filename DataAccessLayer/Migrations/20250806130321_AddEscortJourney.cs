using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddEscortJourney : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "escort_journey",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_in_group_id = table.Column<int>(type: "integer", nullable: true),
                    start_point = table.Column<string>(type: "text", nullable: false),
                    end_point = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expected_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expected_end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    arrival_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    deviation_alert_sent = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escort_journey", x => x.id);
                    table.ForeignKey(
                        name: "FK_escort_journey_account_user_id",
                        column: x => x.user_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_escort_journey_escort_journey_group_created_in_group_id",
                        column: x => x.created_in_group_id,
                        principalTable: "escort_journey_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "current_user_location",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    escort_journey_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    speed = table.Column<decimal>(type: "numeric", nullable: true),
                    heading = table.Column<decimal>(type: "numeric", nullable: true),
                    battery_level = table.Column<int>(type: "integer", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "escort_journey_watcher",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    escort_journey_id = table.Column<int>(type: "integer", nullable: false),
                    watcher_id = table.Column<Guid>(type: "uuid", nullable: false),
                    added_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escort_journey_watcher", x => x.id);
                    table.ForeignKey(
                        name: "FK_escort_journey_watcher_account_watcher_id",
                        column: x => x.watcher_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_escort_journey_watcher_escort_journey_escort_journey_id",
                        column: x => x.escort_journey_id,
                        principalTable: "escort_journey",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "location_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    escort_journey_id = table.Column<int>(type: "integer", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_location_history_escort_journey_escort_journey_id",
                        column: x => x.escort_journey_id,
                        principalTable: "escort_journey",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sos_alert",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    escort_journey_id = table.Column<int>(type: "integer", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lat = table.Column<decimal>(type: "numeric", nullable: false),
                    lng = table.Column<decimal>(type: "numeric", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sos_alert", x => x.id);
                    table.ForeignKey(
                        name: "FK_sos_alert_account_sender_id",
                        column: x => x.sender_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sos_alert_escort_journey_escort_journey_id",
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

            migrationBuilder.CreateIndex(
                name: "IX_escort_journey_created_in_group_id",
                table: "escort_journey",
                column: "created_in_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_escort_journey_user_id",
                table: "escort_journey",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_escort_journey_watcher_escort_journey_id_watcher_id",
                table: "escort_journey_watcher",
                columns: new[] { "escort_journey_id", "watcher_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_escort_journey_watcher_watcher_id",
                table: "escort_journey_watcher",
                column: "watcher_id");

            migrationBuilder.CreateIndex(
                name: "IX_location_history_escort_journey_id",
                table: "location_history",
                column: "escort_journey_id");

            migrationBuilder.CreateIndex(
                name: "IX_sos_alert_escort_journey_id",
                table: "sos_alert",
                column: "escort_journey_id");

            migrationBuilder.CreateIndex(
                name: "IX_sos_alert_sender_id",
                table: "sos_alert",
                column: "sender_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "current_user_location");

            migrationBuilder.DropTable(
                name: "escort_journey_watcher");

            migrationBuilder.DropTable(
                name: "location_history");

            migrationBuilder.DropTable(
                name: "sos_alert");

            migrationBuilder.DropTable(
                name: "escort_journey");
        }
    }
}
