using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_approved",
                table: "escort_journey_group_member");

            migrationBuilder.CreateTable(
                name: "escort_group_join_request",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_approved = table.Column<bool>(type: "boolean", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escort_group_join_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_escort_group_join_request_account_account_id",
                        column: x => x.account_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_escort_group_join_request_escort_journey_group_group_id",
                        column: x => x.group_id,
                        principalTable: "escort_journey_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_escort_group_join_request_account_id",
                table: "escort_group_join_request",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_escort_group_join_request_group_id_account_id",
                table: "escort_group_join_request",
                columns: new[] { "group_id", "account_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "escort_group_join_request");

            migrationBuilder.AddColumn<bool>(
                name: "is_approved",
                table: "escort_journey_group_member",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
