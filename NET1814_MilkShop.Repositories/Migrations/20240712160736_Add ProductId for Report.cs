using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIdforReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "title",
                table: "reports");

            migrationBuilder.AddColumn<Guid>(
                name: "product_id",
                table: "reports",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_reports_product_id",
                table: "reports",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "FK_reports_products_product_id",
                table: "reports",
                column: "product_id",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reports_products_product_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "IX_reports_product_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "product_id",
                table: "reports");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "reports",
                type: "nvarchar(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "reports",
                type: "nvarchar(255)",
                nullable: false,
                defaultValue: "");
        }
    }
}
