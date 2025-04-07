using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialFormType_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDesignable",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "FormTypeId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "ProductForms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FormsTypes",
                columns: table => new
                {
                    TypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FormsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsTypes", x => x.TypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_FormTypeId",
                table: "Products",
                column: "FormTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductForms_TypeId",
                table: "ProductForms",
                column: "TypeId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductForms_FormsTypes_TypeId",
                table: "ProductForms");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_FormsTypes_FormTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "FormsTypes");

            migrationBuilder.DropIndex(
                name: "IX_Products_FormTypeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductForms_TypeId",
                table: "ProductForms");

            migrationBuilder.DropColumn(
                name: "FormTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "ProductForms");

            migrationBuilder.AddColumn<bool>(
                name: "IsDesignable",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
