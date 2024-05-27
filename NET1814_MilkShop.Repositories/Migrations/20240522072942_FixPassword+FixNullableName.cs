using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class FixPasswordFixNullableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_ProductStatuses_status_id",
                table: "products"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_ProductStatuses", table: "ProductStatuses");

            migrationBuilder.RenameTable(name: "ProductStatuses", newName: "product_statuses");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "users",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "products",
                type: "nvarchar(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "product_statuses",
                type: "nvarchar(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldNullable: true
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_statuses",
                table: "product_statuses",
                column: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_products_product_statuses_status_id",
                table: "products",
                column: "status_id",
                principalTable: "product_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_product_statuses_status_id",
                table: "products"
            );

            migrationBuilder.DropPrimaryKey(name: "PK_product_statuses", table: "product_statuses");

            migrationBuilder.RenameTable(name: "product_statuses", newName: "ProductStatuses");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "users",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "products",
                type: "nvarchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ProductStatuses",
                type: "nvarchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)"
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductStatuses",
                table: "ProductStatuses",
                column: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_products_ProductStatuses_status_id",
                table: "products",
                column: "status_id",
                principalTable: "ProductStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
