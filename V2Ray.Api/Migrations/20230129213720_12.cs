using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReturnMoney",
                table: "ProblemReports",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnMoney",
                table: "ProblemReports");
        }
    }
}
