using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountIdentityCardRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_citizen_identity_card_id",
                table: "account");

            migrationBuilder.AlterColumn<string>(
                name: "device_id",
                table: "account",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_citizen_identity_card_user_id",
                table: "citizen_identity_card",
                column: "user_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_citizen_identity_card_account_user_id",
                table: "citizen_identity_card",
                column: "user_id",
                principalTable: "account",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_citizen_identity_card_account_user_id",
                table: "citizen_identity_card");

            migrationBuilder.DropIndex(
                name: "IX_citizen_identity_card_user_id",
                table: "citizen_identity_card");

            migrationBuilder.AlterColumn<string>(
                name: "device_id",
                table: "account",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_account_citizen_identity_card_id",
                table: "account",
                column: "id",
                principalTable: "citizen_identity_card",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
