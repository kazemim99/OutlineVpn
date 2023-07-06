using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "V2Servers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_V2Servers_UserId",
                table: "V2Servers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_V2Servers_Users_UserId",
                table: "V2Servers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_V2Servers_Users_UserId",
                table: "V2Servers");

            migrationBuilder.DropIndex(
                name: "IX_V2Servers_UserId",
                table: "V2Servers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "V2Servers");
        }
    }
}
