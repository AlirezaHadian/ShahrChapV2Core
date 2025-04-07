using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOnProductType_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductForms_ProductFormTypes_PF_TypeId",
                table: "ProductForms");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductFormTypes_FormTypeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductForms_PF_TypeId",
                table: "ProductForms");

            migrationBuilder.DropColumn(
                name: "PF_TypeId",
                table: "ProductForms");

            migrationBuilder.RenameColumn(
                name: "FormTypeId",
                table: "Products",
                newName: "ProductTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_FormTypeId",
                table: "Products",
                newName: "IX_Products_ProductTypeId");

            migrationBuilder.RenameColumn(
                name: "PF_TypeId",
                table: "ProductFormTypes",
                newName: "ProductTypeId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDesignable",
                table: "ProductForms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductFormTypes_ProductTypeId",
                table: "Products",
                column: "ProductTypeId",
                principalTable: "ProductFormTypes",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductFormTypes_ProductTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDesignable",
                table: "ProductForms");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "Products",
                newName: "FormTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ProductTypeId",
                table: "Products",
                newName: "IX_Products_FormTypeId");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "ProductFormTypes",
                newName: "PF_TypeId");

            migrationBuilder.AddColumn<int>(
                name: "PF_TypeId",
                table: "ProductForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductForms_PF_TypeId",
                table: "ProductForms",
                column: "PF_TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductForms_ProductFormTypes_PF_TypeId",
                table: "ProductForms",
                column: "PF_TypeId",
                principalTable: "ProductFormTypes",
                principalColumn: "PF_TypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductFormTypes_FormTypeId",
                table: "Products",
                column: "FormTypeId",
                principalTable: "ProductFormTypes",
                principalColumn: "PF_TypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
