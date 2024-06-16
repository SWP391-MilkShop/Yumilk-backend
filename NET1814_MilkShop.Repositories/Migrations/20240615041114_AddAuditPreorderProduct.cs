using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditPreorderProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "preorder_product",
                type: "datetime2",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "preorder_product",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "preorder_product",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "preorder_product");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "preorder_product");

            migrationBuilder.DropColumn(
                name: "modified_at",
                table: "preorder_product");
        }
    }
}
