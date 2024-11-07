using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WireGuardFilePath",
                table: "SSHKeyInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WireGuardPrivateKey",
                table: "SSHKeyInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WireGuardPublicKey",
                table: "SSHKeyInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WireGuardQRCode",
                table: "SSHKeyInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WireGuardFilePath",
                table: "SSHKeyInfos");

            migrationBuilder.DropColumn(
                name: "WireGuardPrivateKey",
                table: "SSHKeyInfos");

            migrationBuilder.DropColumn(
                name: "WireGuardPublicKey",
                table: "SSHKeyInfos");

            migrationBuilder.DropColumn(
                name: "WireGuardQRCode",
                table: "SSHKeyInfos");
        }
    }
}
