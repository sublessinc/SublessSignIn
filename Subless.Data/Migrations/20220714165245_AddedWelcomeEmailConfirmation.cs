using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Subless.Data.Migrations
{
    public partial class AddedWelcomeEmailConfirmation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WelcomeEmailSent",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.Sql(
    "UPDATE public.\"Users\" " +
    "SET \"WelcomeEmailSent\" = true");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WelcomeEmailSent",
                table: "Users");
        }
    }
}
