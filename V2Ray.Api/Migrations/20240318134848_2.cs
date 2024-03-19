using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedConfirm",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NeedConfirm",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
