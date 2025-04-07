using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ProductFormType_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductForms_FormsTypes_TypeId",
                table: "ProductForms");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_FormsTypes_FormTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "FormsTypes");

            migrationBuilder.DropColumn(
                name: "IsDesigned",
                table: "ProductForms");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                table: "ProductForms",
                newName: "PF_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductForms_TypeId",
                table: "ProductForms",
                newName: "IX_ProductForms_PF_TypeId");

            migrationBuilder.CreateTable(
                name: "ProductFormTypes",
                columns: table => new
                {
                    PF_TypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FormsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFormTypes", x => x.PF_TypeId);
                });

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
                principalColumn: "PF_TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductForms_ProductFormTypes_PF_TypeId",
                table: "ProductForms");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductFormTypes_FormTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductFormTypes");

            migrationBuilder.RenameColumn(
                name: "PF_TypeId",
                table: "ProductForms",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductForms_PF_TypeId",
                table: "ProductForms",
                newName: "IX_ProductForms_TypeId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDesigned",
                table: "ProductForms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FormsTypes",
                columns: table => new
                {
                    TypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormsCount = table.Column<int>(type: "int", nullable: false),
                    TypeTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsTypes", x => x.TypeId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductForms_FormsTypes_TypeId",
                table: "ProductForms",
                column: "TypeId",
                principalTable: "FormsTypes",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_FormsTypes_FormTypeId",
                table: "Products",
                column: "FormTypeId",
                principalTable: "FormsTypes",
                principalColumn: "TypeId");
        }
    }
}
