using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class MiniUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "access_token",
                table: "users");

            migrationBuilder.AddColumn<int>(
                name: "status_id",
                table: "products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "payment_date",
                table: "orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_method",
                table: "orders",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_products_status_id",
                table: "products",
                column: "status_id");

            migrationBuilder.AddForeignKey(
                name: "FK_products_ProductStatuses_status_id",
                table: "products",
                column: "status_id",
                principalTable: "ProductStatuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_products_ProductStatuses_status_id",
                table: "products");

            migrationBuilder.DropTable(
                name: "ProductStatuses");

            migrationBuilder.DropIndex(
                name: "IX_products_status_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "status_id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "payment_date",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "payment_method",
                table: "orders");

            migrationBuilder.AddColumn<string>(
                name: "access_token",
                table: "users",
                type: "nvarchar(255)",
                nullable: true);
        }
    }
}
