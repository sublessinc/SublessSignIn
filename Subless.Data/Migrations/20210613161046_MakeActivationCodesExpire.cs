using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Subless.Data.Migrations
{
    public partial class MakeActivationCodesExpire : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Hits",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivationExpiration",
                table: "Creators",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Users_CognitoId",
                table: "Users",
                column: "CognitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_CognitoAppClientId",
                table: "Partners",
                column: "CognitoAppClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Hits_TimeStamp",
                table: "Hits",
                column: "TimeStamp");

            migrationBuilder.CreateIndex(
                name: "IX_Hits_UserId",
                table: "Hits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Hits_UserId1",
                table: "Hits",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Creators_ActivationCode",
                table: "Creators",
                column: "ActivationCode");

            migrationBuilder.CreateIndex(
                name: "IX_Creators_Username",
                table: "Creators",
                column: "Username");

            migrationBuilder.AddForeignKey(
                name: "FK_Hits_Users_UserId1",
                table: "Hits",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hits_Users_UserId1",
                table: "Hits");

            migrationBuilder.DropIndex(
                name: "IX_Users_CognitoId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Partners_CognitoAppClientId",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Hits_TimeStamp",
                table: "Hits");

            migrationBuilder.DropIndex(
                name: "IX_Hits_UserId",
                table: "Hits");

            migrationBuilder.DropIndex(
                name: "IX_Hits_UserId1",
                table: "Hits");

            migrationBuilder.DropIndex(
                name: "IX_Creators_ActivationCode",
                table: "Creators");

            migrationBuilder.DropIndex(
                name: "IX_Creators_Username",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Hits");

            migrationBuilder.DropColumn(
                name: "ActivationExpiration",
                table: "Creators");
        }
    }
}
