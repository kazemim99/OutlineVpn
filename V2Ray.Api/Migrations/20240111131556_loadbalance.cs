using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class loadbalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasLoadBalance",
                table: "V2Servers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasLoadBalance",
                table: "V2Servers");
        }
    }
}
