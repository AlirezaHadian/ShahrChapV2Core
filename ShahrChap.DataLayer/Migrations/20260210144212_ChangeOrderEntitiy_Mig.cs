using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrderEntitiy_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinally",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinally",
                table: "Orders");
        }
    }
}
