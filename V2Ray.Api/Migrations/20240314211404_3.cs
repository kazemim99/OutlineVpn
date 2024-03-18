using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_ServerId",
                table: "SSHKeyInfos");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "SSHKeyInfos",
                newName: "V2ServerId");


            migrationBuilder.AddForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_V2ServerId",
                table: "SSHKeyInfos",
                column: "V2ServerId",
                principalTable: "V2Servers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_V2ServerId",
                table: "SSHKeyInfos");

            migrationBuilder.RenameColumn(
                name: "V2ServerId",
                table: "SSHKeyInfos",
                newName: "ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_SSHKeyInfos_V2ServerId",
                table: "SSHKeyInfos",
                newName: "IX_SSHKeyInfos_ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_ServerId",
                table: "SSHKeyInfos",
                column: "ServerId",
                principalTable: "V2Servers",
                principalColumn: "Id");
        }
    }
}
