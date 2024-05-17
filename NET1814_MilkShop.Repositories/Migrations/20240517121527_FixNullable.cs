using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class FixNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_order_statuses_status_id",
                table: "orders"
            );

            migrationBuilder.DropForeignKey(name: "FK_orders_vouchers_voucher_id", table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_products_ProductStatuses_status_id",
                table: "products"
            );

            migrationBuilder.DropForeignKey(name: "FK_products_brands_brand_id", table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_category_id",
                table: "products"
            );

            migrationBuilder.DropForeignKey(name: "FK_products_units_unit_id", table: "products");

            migrationBuilder.DropForeignKey(name: "FK_users_roles_role_id", table: "users");

            migrationBuilder.AlterColumn<decimal>(
                name: "min_order_value",
                table: "vouchers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "max_discount_amount",
                table: "vouchers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "vouchers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_percent",
                table: "vouchers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "role_id",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "user_vouchers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "units",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "roles",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "refresh_tokens",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "unit_id",
                table: "products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "sale_price",
                table: "products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "original_price",
                table: "products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "products",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "brand_id",
                table: "products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "product_images",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "product_attributes",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "view_count",
                table: "product_analytics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "purchase_count",
                table: "product_analytics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "product_analytics",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "voucher_id",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "total_price",
                table: "orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "total_amount",
                table: "orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "shipping_fee",
                table: "orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "order_statuses",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "unit_price",
                table: "order_details",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "product_name",
                table: "order_details",
                type: "nvarchar(255)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "item_price",
                table: "order_details",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "order_details",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "order_details",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "customers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "categories",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "carts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "cart_details",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "cart_details",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "brands",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_orders_order_statuses_status_id",
                table: "orders",
                column: "status_id",
                principalTable: "order_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_orders_vouchers_voucher_id",
                table: "orders",
                column: "voucher_id",
                principalTable: "vouchers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_products_ProductStatuses_status_id",
                table: "products",
                column: "status_id",
                principalTable: "ProductStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_products_brands_brand_id",
                table: "products",
                column: "brand_id",
                principalTable: "brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_category_id",
                table: "products",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_products_units_unit_id",
                table: "products",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_role_id",
                table: "users",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_order_statuses_status_id",
                table: "orders"
            );

            migrationBuilder.DropForeignKey(name: "FK_orders_vouchers_voucher_id", table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_products_ProductStatuses_status_id",
                table: "products"
            );

            migrationBuilder.DropForeignKey(name: "FK_products_brands_brand_id", table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_category_id",
                table: "products"
            );

            migrationBuilder.DropForeignKey(name: "FK_products_units_unit_id", table: "products");

            migrationBuilder.DropForeignKey(name: "FK_users_roles_role_id", table: "users");

            migrationBuilder.AlterColumn<decimal>(
                name: "min_order_value",
                table: "vouchers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "max_discount_amount",
                table: "vouchers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "vouchers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "discount_percent",
                table: "vouchers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );

            migrationBuilder.AlterColumn<int>(
                name: "role_id",
                table: "users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "users",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "user_vouchers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "units",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "roles",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "refresh_tokens",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<int>(
                name: "unit_id",
                table: "products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "sale_price",
                table: "products",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "original_price",
                table: "products",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "products",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<int>(
                name: "brand_id",
                table: "products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "product_images",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "product_attributes",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<int>(
                name: "view_count",
                table: "product_analytics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<int>(
                name: "purchase_count",
                table: "product_analytics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "product_analytics",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<int>(
                name: "voucher_id",
                table: "orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "total_price",
                table: "orders",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "total_amount",
                table: "orders",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );

            migrationBuilder.AlterColumn<int>(
                name: "status_id",
                table: "orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "shipping_fee",
                table: "orders",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "order_statuses",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "unit_price",
                table: "order_details",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "product_name",
                table: "order_details",
                type: "nvarchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)"
            );

            migrationBuilder.AlterColumn<decimal>(
                name: "item_price",
                table: "order_details",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "order_details",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "order_details",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "customers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "categories",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "carts",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "cart_details",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "cart_details",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "brands",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_orders_order_statuses_status_id",
                table: "orders",
                column: "status_id",
                principalTable: "order_statuses",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_orders_vouchers_voucher_id",
                table: "orders",
                column: "voucher_id",
                principalTable: "vouchers",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_products_ProductStatuses_status_id",
                table: "products",
                column: "status_id",
                principalTable: "ProductStatuses",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_products_brands_brand_id",
                table: "products",
                column: "brand_id",
                principalTable: "brands",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_category_id",
                table: "products",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_products_units_unit_id",
                table: "products",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "Id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_users_roles_role_id",
                table: "users",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "Id"
            );
        }
    }
}
