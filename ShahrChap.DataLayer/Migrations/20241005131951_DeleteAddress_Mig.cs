using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class DeleteAddress_Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "UserAddresses",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "UserAddresses");
        }
    }
}
