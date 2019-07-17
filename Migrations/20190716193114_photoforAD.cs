using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class photoforAD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhotoForAd",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Url = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    PublicID = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    AdId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoForAd", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoForAd_Advertisement_AdId",
                        column: x => x.AdId,
                        principalTable: "Advertisement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoForAd_AdId",
                table: "PhotoForAd",
                column: "AdId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoForAd");
        }
    }
}
