using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProductTypeId_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductFormTypes_FormTypeId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "FormTypeId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductFormTypes_FormTypeId",
                table: "Products",
                column: "FormTypeId",
                principalTable: "ProductFormTypes",
                principalColumn: "PF_TypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductFormTypes_FormTypeId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "FormTypeId",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductFormTypes_FormTypeId",
                table: "Products",
                column: "FormTypeId",
                principalTable: "ProductFormTypes",
                principalColumn: "PF_TypeId");
        }
    }
}
