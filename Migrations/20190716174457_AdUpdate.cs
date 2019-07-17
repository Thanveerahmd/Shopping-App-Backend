using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class AdUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "timestamp",
                table: "PhotoForUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "timestamp",
                table: "Advertisement",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "PhotoForUsers");

            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "Advertisement");
        }
    }
}
