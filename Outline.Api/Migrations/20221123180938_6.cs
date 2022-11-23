using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Outline.Api.Migrations
{
    public partial class _6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RemainigCapacity",
                table: "Users",
                newName: "CunsumedTraffic");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CunsumedTraffic",
                table: "Users",
                newName: "RemainigCapacity");
        }
    }
}
