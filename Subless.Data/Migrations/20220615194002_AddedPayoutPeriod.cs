using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Subless.Data.Migrations
{
    public partial class AddedPayoutPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PaymentPeriodEnd",
                table: "PaymentAuditLogs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PaymentPeriodStart",
                table: "PaymentAuditLogs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.Sql("UPDATE public.\"PaymentAuditLogs\" SET  \"PaymentPeriodEnd\" = \"DatePaid\" WHERE \"DatePaid\" > '2022-05-06 23:15:54.293819+00'");
            migrationBuilder.Sql("UPDATE public.\"PaymentAuditLogs\" SET  \"PaymentPeriodStart\" = '2022-05-06 23:15:00.00+00' WHERE \"DatePaid\" > '2022-05-06 23:15:54.293819+00'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentPeriodEnd",
                table: "PaymentAuditLogs");

            migrationBuilder.DropColumn(
                name: "PaymentPeriodStart",
                table: "PaymentAuditLogs");
        }
    }
}
