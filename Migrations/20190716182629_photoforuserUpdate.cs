using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class photoforuserUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "timestamp",
                table: "PhotoForUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "timestamp",
                table: "PhotoForUsers",
                nullable: false,
                defaultValue: 0);
        }
    }
}
