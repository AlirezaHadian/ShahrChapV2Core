using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOnModels_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Services",
                newName: "ServiceTitle");

            migrationBuilder.AddColumn<string>(
                name: "ImageTitle",
                table: "ProductGalleries",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "FeatureValues",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageTitle",
                table: "ProductGalleries");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "FeatureValues");

            migrationBuilder.RenameColumn(
                name: "ServiceTitle",
                table: "Services",
                newName: "Title");
        }
    }
}
