using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class FixNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_ParentId",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "vouchers",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Percent",
                table: "vouchers",
                newName: "percent");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "vouchers",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "vouchers",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "users",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "units",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "units",
                newName: "description");

            migrationBuilder.RenameIndex(
                name: "IX_units_Name",
                table: "units",
                newName: "IX_units_name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "roles",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "roles",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "report_types",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "report_types",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "product_statuses",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Review",
                table: "product_reviews",
                newName: "review");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "product_reviews",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "product_attributes",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "product_attributes",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "product_attribute_values",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "orders",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "orders",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "order_statuses",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "order_statuses",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "order_details",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "categories",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "categories",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "categories",
                newName: "parent_id");

            migrationBuilder.RenameIndex(
                name: "IX_categories_Name",
                table: "categories",
                newName: "IX_categories_name");

            migrationBuilder.RenameIndex(
                name: "IX_categories_ParentId",
                table: "categories",
                newName: "IX_categories_parent_id");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "cart_details",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "brands",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "brands",
                newName: "description");

            migrationBuilder.RenameIndex(
                name: "IX_brands_Name",
                table: "brands",
                newName: "IX_brands_name");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_parent_id",
                table: "categories",
                column: "parent_id",
                principalTable: "categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_parent_id",
                table: "categories");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "vouchers",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "percent",
                table: "vouchers",
                newName: "Percent");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "vouchers",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "vouchers",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "units",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "units",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_units_name",
                table: "units",
                newName: "IX_units_Name");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "roles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "roles",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "report_types",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "report_types",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "product_statuses",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "review",
                table: "product_reviews",
                newName: "Review");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "product_reviews",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "product_attributes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "product_attributes",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "product_attribute_values",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "orders",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "orders",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "order_statuses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "order_statuses",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "order_details",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "categories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "categories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "parent_id",
                table: "categories",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_categories_name",
                table: "categories",
                newName: "IX_categories_Name");

            migrationBuilder.RenameIndex(
                name: "IX_categories_parent_id",
                table: "categories",
                newName: "IX_categories_ParentId");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "cart_details",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "brands",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "brands",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_brands_name",
                table: "brands",
                newName: "IX_brands_Name");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_ParentId",
                table: "categories",
                column: "ParentId",
                principalTable: "categories",
                principalColumn: "Id");
        }
    }
}
