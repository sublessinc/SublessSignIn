using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Subless.Data.Migrations
{
    public partial class AddedStripeSync : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StripeSyncs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DateQueued = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsProcessing = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StripeSyncs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StripeSyncs");
        }
    }
}
