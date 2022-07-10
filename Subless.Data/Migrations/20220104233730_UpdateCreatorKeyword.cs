using Microsoft.EntityFrameworkCore.Migrations;

namespace Subless.Data.Migrations
{
    public partial class UpdateCreatorKeyword : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE public.\"Partners\" SET  \"UserPattern\" = REPLACE(\"UserPattern\", 'creator', '{creator}')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
