using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Subless.Data.Migrations
{
    public partial class AddedIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Hits_TimeStamp_CognitoId",
                table: "Hits",
                columns: new[] { "TimeStamp", "CognitoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Hits_TimeStamp_CreatorId",
                table: "Hits",
                columns: new[] { "TimeStamp", "CreatorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Hits_TimeStamp_PartnerId",
                table: "Hits",
                columns: new[] { "TimeStamp", "PartnerId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Hits_TimeStamp_CognitoId",
                table: "Hits");

            migrationBuilder.DropIndex(
                name: "IX_Hits_TimeStamp_CreatorId",
                table: "Hits");

            migrationBuilder.DropIndex(
                name: "IX_Hits_TimeStamp_PartnerId",
                table: "Hits");
        }
    }
}
