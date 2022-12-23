using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Capacity",
                table: "V2Keys",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ClientKeyId",
                table: "V2Keys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ExpireDate",
                table: "V2Keys",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Protocol",
                table: "V2Keys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "V2Keys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "State",
                table: "V2Keys",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "V2Keys");

            migrationBuilder.DropColumn(
                name: "ClientKeyId",
                table: "V2Keys");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "V2Keys");

            migrationBuilder.DropColumn(
                name: "Protocol",
                table: "V2Keys");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "V2Keys");

            migrationBuilder.DropColumn(
                name: "State",
                table: "V2Keys");
        }
    }
}
