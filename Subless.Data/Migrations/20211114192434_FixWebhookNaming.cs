using Microsoft.EntityFrameworkCore.Migrations;

namespace Subless.Data.Migrations
{
    public partial class FixWebhookNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatorActivatedWebhook",
                table: "Partners",
                newName: "CreatorWebhook");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatorWebhook",
                table: "Partners",
                newName: "CreatorActivatedWebhook");
        }
    }
}
