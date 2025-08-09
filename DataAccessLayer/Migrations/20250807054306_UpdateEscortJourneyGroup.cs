using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEscortJourneyGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "max_member_number",
                table: "escort_journey_group",
                type: "character varying(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "end_latitude",
                table: "escort_journey",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "end_longitude",
                table: "escort_journey",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "start_latitude",
                table: "escort_journey",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "start_longitude",
                table: "escort_journey",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_latitude",
                table: "escort_journey");

            migrationBuilder.DropColumn(
                name: "end_longitude",
                table: "escort_journey");

            migrationBuilder.DropColumn(
                name: "start_latitude",
                table: "escort_journey");

            migrationBuilder.DropColumn(
                name: "start_longitude",
                table: "escort_journey");

            migrationBuilder.AlterColumn<string>(
                name: "max_member_number",
                table: "escort_journey_group",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)");
        }
    }
}
