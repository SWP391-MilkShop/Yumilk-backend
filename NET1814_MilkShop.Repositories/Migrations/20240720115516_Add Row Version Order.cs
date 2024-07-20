using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_preorder_product_products_product_id",
                table: "preorder_product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_preorder_product",
                table: "preorder_product");

            migrationBuilder.RenameTable(
                name: "preorder_product",
                newName: "preorder_products");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "product_statuses",
                newName: "name");

            migrationBuilder.AddColumn<byte[]>(
                name: "version",
                table: "orders",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_preorder_products",
                table: "preorder_products",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "FK_preorder_products_products_product_id",
                table: "preorder_products",
                column: "product_id",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_preorder_products_products_product_id",
                table: "preorder_products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_preorder_products",
                table: "preorder_products");

            migrationBuilder.DropColumn(
                name: "version",
                table: "orders");

            migrationBuilder.RenameTable(
                name: "preorder_products",
                newName: "preorder_product");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "product_statuses",
                newName: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_preorder_product",
                table: "preorder_product",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "FK_preorder_product_products_product_id",
                table: "preorder_product",
                column: "product_id",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
