﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class DelivaryInfoWithOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryInfoId",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryInfoId",
                table: "Orders",
                column: "DeliveryInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryInfo_DeliveryInfoId",
                table: "Orders",
                column: "DeliveryInfoId",
                principalTable: "DeliveryInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryInfo_DeliveryInfoId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryInfoId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryInfoId",
                table: "Orders");
        }
    }
}
