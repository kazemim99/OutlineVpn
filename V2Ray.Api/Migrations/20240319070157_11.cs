using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OneAccountPrice",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThreeAccountPrice",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TwoAccountPrice",
                table: "Users",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OneAccountPrice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ThreeAccountPrice",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TwoAccountPrice",
                table: "Users");
        }
    }
}
