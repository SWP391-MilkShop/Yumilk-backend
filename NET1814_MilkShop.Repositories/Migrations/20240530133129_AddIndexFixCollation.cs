using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexFixCollation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "products",
                type: "nvarchar(255)",
                nullable: false,
                collation: "Latin1_General_CI_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldCollation: "Latin1_General_CS_AI");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "products",
                type: "nvarchar(MAX)",
                nullable: true,
                collation: "Latin1_General_CI_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)",
                oldNullable: true,
                oldCollation: "Latin1_General_CS_AI");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "categories",
                type: "nvarchar(255)",
                nullable: false,
                collation: "Latin1_General_CI_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldCollation: "Latin1_General_CS_AI");

            migrationBuilder.CreateIndex(
                name: "IX_units_Name",
                table: "units",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_categories_Name",
                table: "categories",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_brands_Name",
                table: "brands",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_units_Name",
                table: "units");

            migrationBuilder.DropIndex(
                name: "IX_categories_Name",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_brands_Name",
                table: "brands");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "products",
                type: "nvarchar(255)",
                nullable: false,
                collation: "Latin1_General_CS_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldCollation: "Latin1_General_CI_AI");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "products",
                type: "nvarchar(MAX)",
                nullable: true,
                collation: "Latin1_General_CS_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)",
                oldNullable: true,
                oldCollation: "Latin1_General_CI_AI");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "categories",
                type: "nvarchar(255)",
                nullable: false,
                collation: "Latin1_General_CS_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldCollation: "Latin1_General_CI_AI");
        }
    }
}
