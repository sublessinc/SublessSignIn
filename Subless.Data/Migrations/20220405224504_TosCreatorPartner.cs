using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Subless.Data.Migrations
{
    public partial class TosCreatorPartner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptedTerms",
                table: "Partners",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AcceptedTerms",
                table: "Creators",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedTerms",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "AcceptedTerms",
                table: "Creators");
        }
    }
}
