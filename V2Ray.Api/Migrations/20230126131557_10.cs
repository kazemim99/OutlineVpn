using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(name: "ExpireDate", table: "SSHKeyInfos");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "SSHKeyInfos",
                nullable: true,
                type: "timestamp with time zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ExpireDate", table: "SSHKeyInfos")
                 ;
        }
    }
}
