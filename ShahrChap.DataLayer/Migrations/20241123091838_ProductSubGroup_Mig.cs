using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ProductSubGroup_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductGroups_ProductGroupGroupId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductGroupGroupId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductGroupGroupId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "SubGroupId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_GroupId",
                table: "Products",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SubGroupId",
                table: "Products",
                column: "SubGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductGroups_GroupId",
                table: "Products",
                column: "GroupId",
                principalTable: "ProductGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductGroups_SubGroupId",
                table: "Products",
                column: "SubGroupId",
                principalTable: "ProductGroups",
                principalColumn: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductGroups_GroupId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductGroups_SubGroupId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_GroupId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SubGroupId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubGroupId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ProductGroupGroupId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductGroupGroupId",
                table: "Products",
                column: "ProductGroupGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductGroups_ProductGroupGroupId",
                table: "Products",
                column: "ProductGroupGroupId",
                principalTable: "ProductGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
