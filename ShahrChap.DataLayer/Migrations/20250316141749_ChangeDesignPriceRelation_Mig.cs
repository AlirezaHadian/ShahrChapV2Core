using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDesignPriceRelation_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DesignPrices_Products_ProductId",
                table: "DesignPrices");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "DesignPrices",
                newName: "ProductPriceId");

            migrationBuilder.RenameIndex(
                name: "IX_DesignPrices_ProductId",
                table: "DesignPrices",
                newName: "IX_DesignPrices_ProductPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_DesignPrices_ProductPrices_ProductPriceId",
                table: "DesignPrices",
                column: "ProductPriceId",
                principalTable: "ProductPrices",
                principalColumn: "ProductPriceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DesignPrices_ProductPrices_ProductPriceId",
                table: "DesignPrices");

            migrationBuilder.RenameColumn(
                name: "ProductPriceId",
                table: "DesignPrices",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_DesignPrices_ProductPriceId",
                table: "DesignPrices",
                newName: "IX_DesignPrices_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_DesignPrices_Products_ProductId",
                table: "DesignPrices",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
