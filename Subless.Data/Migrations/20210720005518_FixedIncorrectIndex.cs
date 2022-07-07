using Microsoft.EntityFrameworkCore.Migrations;

namespace Subless.Data.Migrations
{
    public partial class FixedIncorrectIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Hits_CognitoId",
                table: "Hits");

            migrationBuilder.CreateIndex(
                name: "IX_Hits_CognitoId",
                table: "Hits",
                column: "CognitoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Hits_CognitoId",
                table: "Hits");

            migrationBuilder.CreateIndex(
                name: "IX_Hits_CognitoId",
                table: "Hits",
                column: "CognitoId",
                unique: true);
        }
    }
}
