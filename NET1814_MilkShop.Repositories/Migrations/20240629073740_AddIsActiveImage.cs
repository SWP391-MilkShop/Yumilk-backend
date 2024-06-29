using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "product_images",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "product_images");
        }
    }
}
