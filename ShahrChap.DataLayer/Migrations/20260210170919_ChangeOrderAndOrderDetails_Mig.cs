using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrderAndOrderDetails_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FeaturesCombination",
                table: "OrderDetails",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AddColumn<string>(
                name: "OrderDetailTitle",
                table: "OrderDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OrderId1",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Services",
                table: "OrderDetails",
                type: "nvarchar(600)",
                maxLength: 600,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId1",
                table: "OrderDetails",
                column: "OrderId1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId1",
                table: "OrderDetails",
                column: "OrderId1",
                principalTable: "Orders",
                principalColumn: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId1",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderId1",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderDetailTitle",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderId1",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Services",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<string>(
                name: "FeaturesCombination",
                table: "OrderDetails",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(600)",
                oldMaxLength: 600);
        }
    }
}
