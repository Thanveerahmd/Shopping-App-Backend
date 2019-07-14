using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class Advertisment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Advertisement",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Url = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    PublicID = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    PaymentStatus = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Advertisement_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BuyerPaymentInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    order_id = table.Column<int>(nullable: false),
                    payment_id = table.Column<int>(nullable: false),
                    payhere_amount = table.Column<float>(nullable: false),
                    payhere_currency = table.Column<string>(nullable: true),
                    status_code = table.Column<int>(nullable: false),
                    method = table.Column<string>(nullable: true),
                    status_message = table.Column<string>(nullable: true),
                    card_holder_name = table.Column<string>(nullable: true),
                    card_no = table.Column<int>(nullable: false),
                    card_expiry = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    DateOfPayment = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuyerPaymentInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BuyerPaymentInfo_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BuyerId = table.Column<string>(nullable: true),
                    Total_Price = table.Column<float>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    PaymentStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SellerPaymentInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    order_id = table.Column<int>(nullable: false),
                    payment_id = table.Column<int>(nullable: false),
                    payhere_amount = table.Column<float>(nullable: false),
                    payhere_currency = table.Column<string>(nullable: true),
                    status_code = table.Column<int>(nullable: false),
                    method = table.Column<string>(nullable: true),
                    status_message = table.Column<string>(nullable: true),
                    card_holder_name = table.Column<string>(nullable: true),
                    card_no = table.Column<int>(nullable: false),
                    card_expiry = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    DateOfPayment = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerPaymentInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SellerPaymentInfo_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    product_Name = table.Column<string>(nullable: true),
                    MainPhotoUrl = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    OrderId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orderDetails_order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_UserId",
                table: "Advertisement",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BuyerPaymentInfo_UserId",
                table: "BuyerPaymentInfo",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_orderDetails_OrderId",
                table: "orderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerPaymentInfo_UserId",
                table: "SellerPaymentInfo",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advertisement");

            migrationBuilder.DropTable(
                name: "BuyerPaymentInfo");

            migrationBuilder.DropTable(
                name: "orderDetails");

            migrationBuilder.DropTable(
                name: "SellerPaymentInfo");

            migrationBuilder.DropTable(
                name: "order");
        }
    }
}
