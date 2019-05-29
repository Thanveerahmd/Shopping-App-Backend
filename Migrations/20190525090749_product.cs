using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Product_name = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    ReorderLevel = table.Column<int>(nullable: false),
                    Price = table.Column<float>(nullable: false),
                    Product_Discription = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    Sub_category = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
