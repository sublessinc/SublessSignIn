using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Subless.Data.Migrations
{
    public partial class AddTargetIdsToHistoricalPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE public.\"PaymentAuditLogs\" " +
                "SET \"TargetId\" = c.\"Id\" " +
                ", \"PayeeType\"=2 " +
                "FROM public.\"Creators\" c " +
                "WHERE c.\"PayPalId\"= public.\"PaymentAuditLogs\".\"PayPalId\"  " +
                "AND public.\"PaymentAuditLogs\".\"TargetId\"='00000000-0000-0000-0000-000000000000'");
            migrationBuilder.Sql(
                "UPDATE public.\"PaymentAuditLogs\"   " +
                "SET \"TargetId\"=p.\"Id\"" +
                ", \"PayeeType\"=1 " +
                "FROM public.\"Partners\"  p " +
                "WHERE p.\"PayPalId\"= public.\"PaymentAuditLogs\".\"PayPalId\" " +
                "AND public.\"PaymentAuditLogs\".\"TargetId\"='00000000-0000-0000-0000-000000000000'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
