using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrderModel_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartToken",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "CheckoutToken",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckoutToken",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "CartToken",
                table: "Orders",
                type: "int",
                nullable: true);
        }
    }
}
