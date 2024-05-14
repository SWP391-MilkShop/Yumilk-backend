using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWP391_DEMO.Migrations
{
    /// <inheritdoc />
    public partial class updateAddress2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "customer_addresses",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "customer_addresses");
        }
    }
}
