using Microsoft.EntityFrameworkCore.Migrations;

namespace Subless.Data.Migrations
{
    public partial class MadeIndexesUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_CognitoId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Partners_CognitoAppClientId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Hits_UserId",
                table: "Hits");

            migrationBuilder.DropIndex(
                name: "IX_Creators_Username",
                table: "Creators");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CognitoId",
                table: "Users",
                column: "CognitoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partners_CognitoAppClientId",
                table: "Partners",
                column: "CognitoAppClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hits_UserId",
                table: "Hits",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Creators_Username_PartnerId",
                table: "Creators",
                columns: new[] { "Username", "PartnerId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_CognitoId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Partners_CognitoAppClientId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Hits_UserId",
                table: "Hits");

            migrationBuilder.DropIndex(
                name: "IX_Creators_Username_PartnerId",
                table: "Creators");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CognitoId",
                table: "Users",
                column: "CognitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_CognitoAppClientId",
                table: "Partners",
                column: "CognitoAppClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Hits_UserId",
                table: "Hits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Creators_Username",
                table: "Creators",
                column: "Username");
        }
    }
}
