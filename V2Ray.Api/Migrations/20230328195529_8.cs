using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "V2Servers",
                type: "int",
                nullable: false,
                defaultValue: 50);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "V2Servers");
        }
    }
}
