using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedTraffic",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "V2Keys",
                newName: "Traffic");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Traffic",
                table: "V2Keys",
                newName: "Capacity");

            migrationBuilder.AddColumn<long>(
                name: "LastUpdatedTraffic",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
