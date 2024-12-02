using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeService_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductServiceId",
                table: "ProductServices",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "ProductServicePriceId",
                table: "ProductServicePrices",
                newName: "ServicePriceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "ProductServices",
                newName: "ProductServiceId");

            migrationBuilder.RenameColumn(
                name: "ServicePriceId",
                table: "ProductServicePrices",
                newName: "ProductServicePriceId");
        }
    }
}
