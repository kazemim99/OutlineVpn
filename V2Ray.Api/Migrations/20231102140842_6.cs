using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        

            migrationBuilder.DropForeignKey(
                name: "FK_V2Servers_Users_UserId",
                table: "V2Servers");

            migrationBuilder.AddForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_ServerId",
                table: "SSHKeyInfos",
                column: "ServerId",
                principalTable: "V2Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_V2Servers_Users_UserId",
                table: "V2Servers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
      

            migrationBuilder.DropForeignKey(
                name: "FK_V2Servers_Users_UserId",
                table: "V2Servers");

            migrationBuilder.AddForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_ServerId",
                table: "SSHKeyInfos",
                column: "ServerId",
                principalTable: "V2Servers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_V2Servers_Users_UserId",
                table: "V2Servers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
