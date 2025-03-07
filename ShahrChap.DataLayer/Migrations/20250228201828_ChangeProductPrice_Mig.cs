using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProductPrice_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Configuration",
                table: "ProductPrices",
                newName: "Combination");

            migrationBuilder.RenameIndex(
                name: "IX_ProductPrices_ProductId_Configuration",
                table: "ProductPrices",
                newName: "IX_ProductPrices_ProductId_Combination");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Combination",
                table: "ProductPrices",
                newName: "Configuration");

            migrationBuilder.RenameIndex(
                name: "IX_ProductPrices_ProductId_Combination",
                table: "ProductPrices",
                newName: "IX_ProductPrices_ProductId_Configuration");
        }
    }
}
