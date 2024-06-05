using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddCollation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "users",
                type: "nvarchar(50)",
                nullable: false,
                collation: "Latin1_General_CS_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "units",
                type: "nvarchar(255)",
                nullable: false,
                collation: "Latin1_General_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "products",
                type: "nvarchar(255)",
                nullable: false,
                collation: "Latin1_General_CS_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "products",
                type: "nvarchar(2000)",
                nullable: true,
                collation: "Latin1_General_CS_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "product_attributes",
                type: "nvarchar(255)",
                nullable: false,
                collation: "Latin1_General_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "order_statuses",
                type: "nvarchar(255)",
                nullable: false,
                collation: "Latin1_General_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "categories",
                type: "nvarchar(255)",
                nullable: false,
                collation: "Latin1_General_CS_AI",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "brands",
                type: "nvarchar(255)",
                nullable: false,
                collation: "Latin1_General_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "users",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldCollation: "Latin1_General_CS_AS"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "units",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldCollation: "Latin1_General_CI_AS"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "products",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldCollation: "Latin1_General_CS_AI"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "products",
                type: "nvarchar(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldNullable: true,
                oldCollation: "Latin1_General_CS_AI"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "product_attributes",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldCollation: "Latin1_General_CI_AS"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "order_statuses",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldCollation: "Latin1_General_CI_AS"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "categories",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldCollation: "Latin1_General_CS_AI"
            );

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "brands",
                type: "nvarchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldCollation: "Latin1_General_CI_AS"
            );
        }
    }
}
