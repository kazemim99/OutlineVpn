using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V2Ray.Api.Migrations
{
    public partial class _7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("ExpireDate", "SSHKeyInfos");

            migrationBuilder.AddColumn<long>(
                name: "ExpireDate",
                table: "SSHKeyInfos",
                type: "bigint",
                nullable: false
              );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("ExpireDate", "SSHKeyInfos");

           
        }
    }
}
