using Microsoft.EntityFrameworkCore.Migrations;

namespace Subless.Data.Migrations
{
    public partial class RenamedPayoneerToPaypal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayoneerId",
                table: "PaymentAuditLogs",
                newName: "PayPalId");

            migrationBuilder.RenameColumn(
                name: "PayoneerId",
                table: "Payee",
                newName: "PayPalId");

            migrationBuilder.RenameColumn(
                name: "PayoneerId",
                table: "Partners",
                newName: "PayPalId");

            migrationBuilder.RenameColumn(
                name: "PayoneerId",
                table: "Creators",
                newName: "PayPalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PayPalId",
                table: "PaymentAuditLogs",
                newName: "PayoneerId");

            migrationBuilder.RenameColumn(
                name: "PayPalId",
                table: "Payee",
                newName: "PayoneerId");

            migrationBuilder.RenameColumn(
                name: "PayPalId",
                table: "Partners",
                newName: "PayoneerId");

            migrationBuilder.RenameColumn(
                name: "PayPalId",
                table: "Creators",
                newName: "PayoneerId");
        }
    }
}
