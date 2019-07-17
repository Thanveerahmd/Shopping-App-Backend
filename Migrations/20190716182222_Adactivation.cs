using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class Adactivation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivationStatus",
                table: "Advertisement",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivationStatus",
                table: "Advertisement");
        }
    }
}
