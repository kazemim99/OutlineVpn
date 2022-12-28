using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_V2Servers_V2ServerId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_V2Keys_V2Servers_ServerId",
                table: "V2Keys");

            migrationBuilder.DropIndex(
                name: "IX_Users_V2ServerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "V2ServerId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ServerId",
                table: "V2Keys",
                newName: "V2ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_V2Keys_ServerId",
                table: "V2Keys",
                newName: "IX_V2Keys_V2ServerId");

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "V2Servers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_V2Keys_V2Servers_V2ServerId",
                table: "V2Keys",
                column: "V2ServerId",
                principalTable: "V2Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_V2Keys_V2Servers_V2ServerId",
                table: "V2Keys");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "V2Servers");

            migrationBuilder.RenameColumn(
                name: "V2ServerId",
                table: "V2Keys",
                newName: "ServerId");

            migrationBuilder.RenameIndex(
                name: "IX_V2Keys_V2ServerId",
                table: "V2Keys",
                newName: "IX_V2Keys_ServerId");

            migrationBuilder.AddColumn<int>(
                name: "V2ServerId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_V2ServerId",
                table: "Users",
                column: "V2ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_V2Servers_V2ServerId",
                table: "Users",
                column: "V2ServerId",
                principalTable: "V2Servers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_V2Keys_V2Servers_ServerId",
                table: "V2Keys",
                column: "ServerId",
                principalTable: "V2Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
