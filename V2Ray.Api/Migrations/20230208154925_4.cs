using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_V2Servers_Cities_CityId",
                table: "V2Servers");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "V2Servers");

            migrationBuilder.DropColumn(
                name: "KeyCount",
                table: "V2Servers");

            migrationBuilder.DropColumn(
                name: "Swapped",
                table: "V2Servers");

            migrationBuilder.RenameColumn(
                name: "IPs",
                table: "V2Servers",
                newName: "IP");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "V2Servers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "SSHKeyInfos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SSHKeyInfos_ServerId",
                table: "SSHKeyInfos",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_ServerId",
                table: "SSHKeyInfos",
                column: "ServerId",
                principalTable: "V2Servers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_V2Servers_Cities_CityId",
                table: "V2Servers",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_ServerId",
                table: "SSHKeyInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_V2Servers_Cities_CityId",
                table: "V2Servers");

            migrationBuilder.DropIndex(
                name: "IX_SSHKeyInfos_ServerId",
                table: "SSHKeyInfos");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "SSHKeyInfos");

            migrationBuilder.RenameColumn(
                name: "IP",
                table: "V2Servers",
                newName: "IPs");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "V2Servers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "V2Servers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KeyCount",
                table: "V2Servers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Swapped",
                table: "V2Servers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_V2Servers_Cities_CityId",
                table: "V2Servers",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
