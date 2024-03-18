using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_ServerId",
                table: "SSHKeyInfos");

            migrationBuilder.AddColumn<int>(
                name: "MultiUser",
                table: "V2Servers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServerId",
                table: "SSHKeyInfos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "MultiUser",
                table: "SSHKeyInfos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_ServerId",
                table: "SSHKeyInfos",
                column: "ServerId",
                principalTable: "V2Servers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_ServerId",
                table: "SSHKeyInfos");

            migrationBuilder.DropColumn(
                name: "MultiUser",
                table: "V2Servers");

            migrationBuilder.DropColumn(
                name: "MultiUser",
                table: "SSHKeyInfos");

            migrationBuilder.AlterColumn<int>(
                name: "ServerId",
                table: "SSHKeyInfos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SSHKeyInfos_V2Servers_ServerId",
                table: "SSHKeyInfos",
                column: "ServerId",
                principalTable: "V2Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
