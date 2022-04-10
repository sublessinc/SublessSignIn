using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Subless.Data.Migrations
{
    public partial class MultiplePartnerUrls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Sites",
                table: "Partners",
                type: "text[]",
                nullable: true);
            migrationBuilder.Sql("UPDATE public.\"Partners\" SET \"Sites\" = string_to_array(\"Site\", ',')");
            migrationBuilder.DropColumn(
                name: "Site",
                table: "Partners");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sites",
                table: "Partners");

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "Partners",
                type: "text",
                nullable: true);
        }
    }
}
