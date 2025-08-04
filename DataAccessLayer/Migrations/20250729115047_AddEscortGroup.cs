using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddEscortGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "escort_journey_group",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    leader_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    group_code = table.Column<string>(type: "text", nullable: false),
                    member_limit_tier = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escort_journey_group", x => x.id);
                    table.ForeignKey(
                        name: "FK_escort_journey_group_account_leader_id",
                        column: x => x.leader_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "escort_journey_group_member",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_online = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escort_journey_group_member", x => x.id);
                    table.ForeignKey(
                        name: "FK_escort_journey_group_member_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_escort_journey_group_member_escort_journey_group_group_id",
                        column: x => x.group_id,
                        principalTable: "escort_journey_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_escort_journey_group_group_code",
                table: "escort_journey_group",
                column: "group_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_escort_journey_group_leader_id",
                table: "escort_journey_group",
                column: "leader_id");

            migrationBuilder.CreateIndex(
                name: "IX_escort_journey_group_member_account_id",
                table: "escort_journey_group_member",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_escort_journey_group_member_group_id_account_id",
                table: "escort_journey_group_member",
                columns: new[] { "group_id", "account_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "escort_journey_group_member");

            migrationBuilder.DropTable(
                name: "escort_journey_group");
        }
    }
}
