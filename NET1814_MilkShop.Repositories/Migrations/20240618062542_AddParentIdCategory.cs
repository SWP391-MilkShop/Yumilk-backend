using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NET1814_MilkShop.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddParentIdCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_categories_ParentId",
                table: "categories",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_categories_categories_ParentId",
                table: "categories",
                column: "ParentId",
                principalTable: "categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_categories_categories_ParentId",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "IX_categories_ParentId",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "categories");
        }
    }
}
