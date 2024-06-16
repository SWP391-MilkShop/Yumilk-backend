using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingCode_Order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "shipping_code",
                table: "orders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shipping_code",
                table: "orders");
        }
    }
}
