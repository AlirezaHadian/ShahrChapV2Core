using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProductType_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductFormTypes_ProductTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductFormTypes");

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    ProductTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FormsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.ProductTypeId);
                });
            migrationBuilder.Sql(@"
SET IDENTITY_INSERT ProductTypes ON;

INSERT INTO ProductTypes (ProductTypeId, TypeTitle, FormsCount)
SELECT DISTINCT ProductTypeId, 'Temporary Title', 0
FROM Products
WHERE ProductTypeId IS NOT NULL AND 
      ProductTypeId NOT IN (SELECT ProductTypeId FROM ProductTypes);

SET IDENTITY_INSERT ProductTypes OFF;
");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductTypes_ProductTypeId",
                table: "Products",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductTypes_ProductTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.CreateTable(
                name: "ProductFormTypes",
                columns: table => new
                {
                    ProductTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormsCount = table.Column<int>(type: "int", nullable: false),
                    TypeTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFormTypes", x => x.ProductTypeId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductFormTypes_ProductTypeId",
                table: "Products",
                column: "ProductTypeId",
                principalTable: "ProductFormTypes",
                principalColumn: "ProductTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
