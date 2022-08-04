using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Subless.Data.Migrations
{
    public partial class AddedIdleEmailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Ticket",
                table: "UserSessions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "UserSessions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "UserSessions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "IdleEmailExecutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodStart = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DateQueued = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsProcessing = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateExecuted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdleEmailExecutions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdleEmailExecutions");

            migrationBuilder.AlterColumn<string>(
                name: "Ticket",
                table: "UserSessions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "SubjectId",
                table: "UserSessions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "UserSessions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
