using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJourneyGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE escort_journey_group 
          ALTER COLUMN max_member_number 
          TYPE integer 
          USING max_member_number::integer;");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE escort_journey_group 
          ALTER COLUMN max_member_number 
          TYPE character varying(64) 
          USING max_member_number::text;");
        }

    }
}
