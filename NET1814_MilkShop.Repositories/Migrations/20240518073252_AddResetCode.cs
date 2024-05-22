using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddResetCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "verification_token",
                table: "users",
                newName: "verification_code"
            );

            migrationBuilder.AddColumn<string>(
                name: "reset_password_code",
                table: "users",
                type: "nvarchar(6)",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "reset_password_code", table: "users");

            migrationBuilder.RenameColumn(
                name: "verification_code",
                table: "users",
                newName: "verification_token"
            );
        }
    }
}
