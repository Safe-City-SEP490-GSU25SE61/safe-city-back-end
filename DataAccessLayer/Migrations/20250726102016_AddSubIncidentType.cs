using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddSubIncidentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "environment_sub_category",
                table: "incident_report",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "infrastructure_sub_category",
                table: "incident_report",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "other_sub_category",
                table: "incident_report",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "security_sub_category",
                table: "incident_report",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "traffic_sub_category",
                table: "incident_report",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "environment_sub_category",
                table: "incident_report");

            migrationBuilder.DropColumn(
                name: "infrastructure_sub_category",
                table: "incident_report");

            migrationBuilder.DropColumn(
                name: "other_sub_category",
                table: "incident_report");

            migrationBuilder.DropColumn(
                name: "security_sub_category",
                table: "incident_report");

            migrationBuilder.DropColumn(
                name: "traffic_sub_category",
                table: "incident_report");
        }
    }
}
