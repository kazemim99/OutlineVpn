using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientKeyId",
                table: "V2Keys");

            migrationBuilder.AddColumn<int>(
                name: "ClientPort",
                table: "V2Keys",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientPort",
                table: "V2Keys");

            migrationBuilder.AddColumn<string>(
                name: "ClientKeyId",
                table: "V2Keys",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
