using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Subless.Data.Migrations
{
    public partial class AddedFeesDetailsToPayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Fees",
                table: "PaymentAuditLogs",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PayeeType",
                table: "PaymentAuditLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Revenue",
                table: "PaymentAuditLogs",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "PaymentAuditLogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "PayeeType",
                table: "Payee",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "Payee",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fees",
                table: "PaymentAuditLogs");

            migrationBuilder.DropColumn(
                name: "PayeeType",
                table: "PaymentAuditLogs");

            migrationBuilder.DropColumn(
                name: "Revenue",
                table: "PaymentAuditLogs");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "PaymentAuditLogs");

            migrationBuilder.DropColumn(
                name: "PayeeType",
                table: "Payee");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Payee");
        }
    }
}
