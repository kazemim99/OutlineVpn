using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Outline.Api.Migrations
{
    public partial class _16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ServerId",
                table: "Users",
                column: "ServerId");

        

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ApiUrls_ServerId",
                table: "Users",
                column: "ServerId",
                principalTable: "ApiUrls",
                principalColumn: "Id");

      
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_ApiUrls_ServerId",
                table: "Users");


            migrationBuilder.DropIndex(
                name: "IX_Users_ServerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "Users");

  
        }
    }
}
