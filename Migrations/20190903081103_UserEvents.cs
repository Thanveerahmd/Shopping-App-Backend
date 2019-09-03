using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class UserEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuyerSearch",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    Keyword = table.Column<string>(nullable: true),
                    NoOfSearch = table.Column<int>(nullable: false),
                    LatestVisit = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerSearch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyerSearch_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PageViews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    Sub_category = table.Column<string>(nullable: true),
                    Sub_categoryId = table.Column<int>(nullable: false),
                    NoOfVisits = table.Column<int>(nullable: false),
                    LatestVisit = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageViews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BuyerSearch_UserId",
                table: "BuyerSearch",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageViews_UserId",
                table: "PageViews",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuyerSearch");

            migrationBuilder.DropTable(
                name: "PageViews");
        }
    }
}
