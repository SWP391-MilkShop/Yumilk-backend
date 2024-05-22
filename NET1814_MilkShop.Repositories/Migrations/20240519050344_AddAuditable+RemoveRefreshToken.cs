using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditableRemoveRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "refresh_tokens");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "vouchers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "vouchers",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "users",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_vouchers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "user_vouchers",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "units",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "units",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "roles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "roles",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "ProductStatuses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "ProductStatuses",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "ProductStatuses",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "products",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "product_images",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "product_images",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "product_attributes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "product_attributes",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "product_attribute_values",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "product_attribute_values",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "product_attribute_values",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "product_analytics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "product_analytics",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "orders",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "order_statuses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "order_statuses",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "order_details",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "order_details",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "customers",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "customers",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "customer_addresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "customer_addresses",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "categories",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "carts",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "cart_details",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "cart_details",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "cart_details",
                type: "datetime2",
                nullable: true
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "brands",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true
            );

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "brands",
                type: "datetime2",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "modified_at", table: "vouchers");

            migrationBuilder.DropColumn(name: "modified_at", table: "users");

            migrationBuilder.DropColumn(name: "modified_at", table: "user_vouchers");

            migrationBuilder.DropColumn(name: "modified_at", table: "units");

            migrationBuilder.DropColumn(name: "modified_at", table: "roles");

            migrationBuilder.DropColumn(name: "created_at", table: "ProductStatuses");

            migrationBuilder.DropColumn(name: "deleted_at", table: "ProductStatuses");

            migrationBuilder.DropColumn(name: "modified_at", table: "ProductStatuses");

            migrationBuilder.DropColumn(name: "modified_at", table: "products");

            migrationBuilder.DropColumn(name: "modified_at", table: "product_images");

            migrationBuilder.DropColumn(name: "modified_at", table: "product_attributes");

            migrationBuilder.DropColumn(name: "created_at", table: "product_attribute_values");

            migrationBuilder.DropColumn(name: "deleted_at", table: "product_attribute_values");

            migrationBuilder.DropColumn(name: "modified_at", table: "product_attribute_values");

            migrationBuilder.DropColumn(name: "modified_at", table: "product_analytics");

            migrationBuilder.DropColumn(name: "modified_at", table: "orders");

            migrationBuilder.DropColumn(name: "modified_at", table: "order_statuses");

            migrationBuilder.DropColumn(name: "created_at", table: "order_details");

            migrationBuilder.DropColumn(name: "modified_at", table: "order_details");

            migrationBuilder.DropColumn(name: "created_at", table: "customers");

            migrationBuilder.DropColumn(name: "deleted_at", table: "customers");

            migrationBuilder.DropColumn(name: "modified_at", table: "customers");

            migrationBuilder.DropColumn(name: "modified_at", table: "customer_addresses");

            migrationBuilder.DropColumn(name: "modified_at", table: "categories");

            migrationBuilder.DropColumn(name: "modified_at", table: "carts");

            migrationBuilder.DropColumn(name: "created_at", table: "cart_details");

            migrationBuilder.DropColumn(name: "deleted_at", table: "cart_details");

            migrationBuilder.DropColumn(name: "modified_at", table: "cart_details");

            migrationBuilder.DropColumn(name: "modified_at", table: "brands");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "vouchers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_vouchers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "units",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "roles",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "products",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "product_images",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "product_attributes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "product_analytics",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "order_statuses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "customer_addresses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "categories",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "carts",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "brands",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2"
            );

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id"
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id"
            );
        }
    }
}
