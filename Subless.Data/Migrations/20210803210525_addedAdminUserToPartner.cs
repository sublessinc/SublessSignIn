using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Subless.Data.Migrations
{
    public partial class addedAdminUserToPartner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Admin",
                table: "Partners",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Partners_Admin",
                table: "Partners",
                column: "Admin");

            migrationBuilder.AddForeignKey(
                name: "FK_Partners_Users_Admin",
                table: "Partners",
                column: "Admin",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partners_Users_Admin",
                table: "Partners");

            migrationBuilder.DropIndex(
                name: "IX_Partners_Admin",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Admin",
                table: "Partners");
        }
    }
}
