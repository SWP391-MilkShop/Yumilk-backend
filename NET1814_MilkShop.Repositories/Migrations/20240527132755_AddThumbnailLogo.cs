using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "products",
                type: "nvarchar(MAX)",
                nullable: true,
                collation: "Latin1_General_CS_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldNullable: true,
                oldCollation: "Latin1_General_CS_AI"
            );

            migrationBuilder.AddColumn<string>(
                name: "thumbnail",
                table: "products",
                type: "nvarchar(255)",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "logo",
                table: "brands",
                type: "nvarchar(255)",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "thumbnail", table: "products");

            migrationBuilder.DropColumn(name: "logo", table: "brands");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "products",
                type: "nvarchar(2000)",
                nullable: true,
                collation: "Latin1_General_CS_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)",
                oldNullable: true,
                oldCollation: "Latin1_General_CS_AI"
            );
        }
    }
}
