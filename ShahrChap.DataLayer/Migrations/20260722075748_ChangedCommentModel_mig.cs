using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShahrChap.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangedCommentModel_mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductCommentCommentID",
                table: "ProductComments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductComments_ProductCommentCommentID",
                table: "ProductComments",
                column: "ProductCommentCommentID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductComments_ProductComments_ProductCommentCommentID",
                table: "ProductComments",
                column: "ProductCommentCommentID",
                principalTable: "ProductComments",
                principalColumn: "CommentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductComments_ProductComments_ProductCommentCommentID",
                table: "ProductComments");

            migrationBuilder.DropIndex(
                name: "IX_ProductComments_ProductCommentCommentID",
                table: "ProductComments");

            migrationBuilder.DropColumn(
                name: "ProductCommentCommentID",
                table: "ProductComments");
        }
    }
}
