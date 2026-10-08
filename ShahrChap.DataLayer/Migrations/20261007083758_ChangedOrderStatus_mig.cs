using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangedOrderStatus_mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrderColor",
                table: "OrderStatuses",
                newName: "StatusIcon");

            migrationBuilder.AddColumn<string>(
                name: "StatusColor",
                table: "OrderStatuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusColor",
                table: "OrderStatuses");

            migrationBuilder.RenameColumn(
                name: "StatusIcon",
                table: "OrderStatuses",
                newName: "OrderColor");
        }
    }
}
