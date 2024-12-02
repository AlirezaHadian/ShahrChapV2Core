using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ServiceChange_Mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductServicePrices_ProductPrices_ProductPriceId",
                table: "ProductServicePrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductServicePrices_ProductServices_ProductServiceId",
                table: "ProductServicePrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductServices_Products_ProductId",
                table: "ProductServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductServices",
                table: "ProductServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductServicePrices",
                table: "ProductServicePrices");

            migrationBuilder.RenameTable(
                name: "ProductServices",
                newName: "Services");

            migrationBuilder.RenameTable(
                name: "ProductServicePrices",
                newName: "ServicePrices");

            migrationBuilder.RenameIndex(
                name: "IX_ProductServices_ProductId",
                table: "Services",
                newName: "IX_Services_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductServicePrices_ProductServiceId",
                table: "ServicePrices",
                newName: "IX_ServicePrices_ProductServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductServicePrices_ProductPriceId",
                table: "ServicePrices",
                newName: "IX_ServicePrices_ProductPriceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "ServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServicePrices",
                table: "ServicePrices",
                column: "ServicePriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePrices_ProductPrices_ProductPriceId",
                table: "ServicePrices",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "ProductPriceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePrices_Services_ProductServiceId",
                table: "ServicePrices",
                column: "ProductServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Products_ProductId",
                table: "Services",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicePrices_ProductPrices_ProductPriceId",
                table: "ServicePrices");

            migrationBuilder.DropForeignKey(
                name: "FK_ServicePrices_Services_ProductServiceId",
                table: "ServicePrices");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Products_ProductId",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServicePrices",
                table: "ServicePrices");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "ProductServices");

            migrationBuilder.RenameTable(
                name: "ServicePrices",
                newName: "ProductServicePrices");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ProductId",
                table: "ProductServices",
                newName: "IX_ProductServices_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ServicePrices_ProductServiceId",
                table: "ProductServicePrices",
                newName: "IX_ProductServicePrices_ProductServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ServicePrices_ProductPriceId",
                table: "ProductServicePrices",
                newName: "IX_ProductServicePrices_ProductPriceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductServices",
                table: "ProductServices",
                column: "ServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductServicePrices",
                table: "ProductServicePrices",
                column: "ServicePriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductServicePrices_ProductPrices_ProductPriceId",
                table: "ProductServicePrices",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "ProductPriceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductServicePrices_ProductServices_ProductServiceId",
                table: "ProductServicePrices",
                column: "ProductServiceId",
                principalTable: "ProductServices",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductServices_Products_ProductId",
                table: "ProductServices",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
