using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIncidentReportNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "incident_report",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    district_id = table.Column<int>(type: "integer", nullable: true),
                    ward_id = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    lat = table.Column<decimal>(type: "numeric", nullable: true),
                    lng = table.Column<decimal>(type: "numeric", nullable: true),
                    is_anonymous = table.Column<bool>(type: "boolean", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    verified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    image_urls = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_incident_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_incident_report_account_user_id",
                        column: x => x.user_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_incident_report_account_verified_by",
                        column: x => x.verified_by,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_incident_report_district_district_id",
                        column: x => x.district_id,
                        principalTable: "district",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_incident_report_ward_ward_id",
                        column: x => x.ward_id,
                        principalTable: "ward",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TransactionCode = table.Column<string>(type: "text", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_account_UserId",
                        column: x => x.UserId,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "subscription",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "note",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    report_id = table.Column<Guid>(type: "uuid", nullable: false),
                    officer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_note", x => x.id);
                    table.ForeignKey(
                        name: "FK_note_account_officer_id",
                        column: x => x.officer_id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_note_incident_report_report_id",
                        column: x => x.report_id,
                        principalTable: "incident_report",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayosTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderCode = table.Column<string>(type: "text", nullable: false),
                    QrCodeUrl = table.Column<string>(type: "text", nullable: false),
                    DeeplinkUrl = table.Column<string>(type: "text", nullable: false),
                    ExpiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    WebhookReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayosTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayosTransactions_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_incident_report_district_id",
                table: "incident_report",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "IX_incident_report_user_id",
                table: "incident_report",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_incident_report_verified_by",
                table: "incident_report",
                column: "verified_by");

            migrationBuilder.CreateIndex(
                name: "IX_incident_report_ward_id",
                table: "incident_report",
                column: "ward_id");

            migrationBuilder.CreateIndex(
                name: "IX_note_officer_id",
                table: "note",
                column: "officer_id");

            migrationBuilder.CreateIndex(
                name: "IX_note_report_id",
                table: "note",
                column: "report_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SubscriptionId",
                table: "Payments",
                column: "SubscriptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PayosTransactions_OrderCode",
                table: "PayosTransactions",
                column: "OrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayosTransactions_PaymentId",
                table: "PayosTransactions",
                column: "PaymentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "note");

            migrationBuilder.DropTable(
                name: "PayosTransactions");

            migrationBuilder.DropTable(
                name: "incident_report");

            migrationBuilder.DropTable(
                name: "Payments");

           
        }
    }
}
