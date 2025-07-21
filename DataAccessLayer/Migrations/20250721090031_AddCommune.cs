using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddCommune : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_district_district_id",
                table: "account");

            migrationBuilder.DropForeignKey(
                name: "FK_incident_report_district_district_id",
                table: "incident_report");

            migrationBuilder.DropForeignKey(
                name: "FK_incident_report_ward_ward_id",
                table: "incident_report");

            migrationBuilder.DropTable(
                name: "ward");

            migrationBuilder.DropIndex(
                name: "IX_incident_report_ward_id",
                table: "incident_report");



            migrationBuilder.DropColumn(
                name: "danger_level",
                table: "district");

            migrationBuilder.RenameTable(
                name: "district",
                newName: "commune");

            migrationBuilder.RenameIndex(
                name: "PK_district",
                table: "commune",
                newName: "PK_commune");


            migrationBuilder.RenameColumn(
                name: "district_id",
                table: "incident_report",
                newName: "commune_id");

            migrationBuilder.RenameIndex(
                name: "IX_incident_report_district_id",
                table: "incident_report",
                newName: "IX_incident_report_commune_id");

            migrationBuilder.RenameColumn(
                name: "district_id",
                table: "account",
                newName: "commune_id");

            migrationBuilder.RenameIndex(
                name: "IX_account_district_id",
                table: "account",
                newName: "IX_account_commune_id");



            migrationBuilder.AddForeignKey(
                name: "FK_account_commune_commune_id",
                table: "account",
                column: "commune_id",
                principalTable: "commune",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_incident_report_commune_commune_id",
                table: "incident_report",
                column: "commune_id",
                principalTable: "commune",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_commune_commune_id",
                table: "account");

            migrationBuilder.DropForeignKey(
                name: "FK_incident_report_commune_commune_id",
                table: "incident_report");

            migrationBuilder.DropPrimaryKey(
                name: "PK_commune",
                table: "commune");

            // Đổi tên bảng commune → district
            migrationBuilder.RenameTable(
                name: "commune",
                newName: "district");

            // Rename lại index PK
            migrationBuilder.RenameIndex(
                name: "PK_commune",
                table: "district",
                newName: "PK_district");

            // Add lại PK
            migrationBuilder.AddPrimaryKey(
                name: "PK_district",
                table: "district",
                column: "id");

            // Đổi lại tên cột trong incident_report
            migrationBuilder.RenameColumn(
                name: "commune_id",
                table: "incident_report",
                newName: "district_id");

            migrationBuilder.RenameIndex(
                name: "IX_incident_report_commune_id",
                table: "incident_report",
                newName: "IX_incident_report_district_id");

            // Đổi lại tên cột trong account
            migrationBuilder.RenameColumn(
                name: "commune_id",
                table: "account",
                newName: "district_id");

            migrationBuilder.RenameIndex(
                name: "IX_account_commune_id",
                table: "account",
                newName: "IX_account_district_id");

            // Thêm lại cột danger_level đã drop ở Up()
            migrationBuilder.AddColumn<int>(
                name: "danger_level",
                table: "district",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Tạo lại bảng ward
            migrationBuilder.CreateTable(
                name: "ward",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    district_id = table.Column<int>(type: "integer", nullable: false),
                    create_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    danger_level = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false),
                    polygon_data = table.Column<string>(type: "text", nullable: false),
                    total_reported_incidents = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ward", x => x.id);
                    table.ForeignKey(
                        name: "FK_ward_district_district_id",
                        column: x => x.district_id,
                        principalTable: "district",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Tạo lại index + FK
            migrationBuilder.CreateIndex(
                name: "IX_incident_report_ward_id",
                table: "incident_report",
                column: "ward_id");

            migrationBuilder.CreateIndex(
                name: "IX_ward_district_id",
                table: "ward",
                column: "district_id");

            migrationBuilder.AddForeignKey(
                name: "FK_account_district_district_id",
                table: "account",
                column: "district_id",
                principalTable: "district",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_incident_report_district_district_id",
                table: "incident_report",
                column: "district_id",
                principalTable: "district",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_incident_report_ward_ward_id",
                table: "incident_report",
                column: "ward_id",
                principalTable: "ward",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

    }
}
