using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialForms_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductForms",
                columns: table => new
                {
                    ProductFormId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDesigned = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductForms", x => x.ProductFormId);
                    table.ForeignKey(
                        name: "FK_ProductForms_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormInputs",
                columns: table => new
                {
                    FormInputId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductFormId = table.Column<int>(type: "int", nullable: false),
                    FeatureValueId = table.Column<int>(type: "int", nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: true),
                    InputName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    InputType = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Options = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormInputs", x => x.FormInputId);
                    table.ForeignKey(
                        name: "FK_FormInputs_FeatureValues_FeatureValueId",
                        column: x => x.FeatureValueId,
                        principalTable: "FeatureValues",
                        principalColumn: "FeatureValueId");
                    table.ForeignKey(
                        name: "FK_FormInputs_ProductForms_ProductFormId",
                        column: x => x.ProductFormId,
                        principalTable: "ProductForms",
                        principalColumn: "ProductFormId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormInputs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_FormInputs_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormInputs_FeatureValueId",
                table: "FormInputs",
                column: "FeatureValueId");

            migrationBuilder.CreateIndex(
                name: "IX_FormInputs_ProductFormId",
                table: "FormInputs",
                column: "ProductFormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormInputs_ProductId",
                table: "FormInputs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FormInputs_ServiceId",
                table: "FormInputs",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductForms_ProductId",
                table: "ProductForms",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormInputs");

            migrationBuilder.DropTable(
                name: "ProductForms");
        }
    }
}
