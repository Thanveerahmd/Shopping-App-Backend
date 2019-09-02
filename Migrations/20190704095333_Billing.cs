using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class Billing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BillingInfo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillingInfo_UserId",
                table: "BillingInfo",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillingInfo_AspNetUsers_UserId",
                table: "BillingInfo",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingInfo_AspNetUsers_UserId",
                table: "BillingInfo");

            migrationBuilder.DropIndex(
                name: "IX_BillingInfo_UserId",
                table: "BillingInfo");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BillingInfo",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
