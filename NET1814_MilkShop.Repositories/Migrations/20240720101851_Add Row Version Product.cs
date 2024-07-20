using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "version",
                table: "products",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "version",
                table: "products");
        }
    }
}
