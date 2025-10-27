using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace July_Team.Migrations
{
    /// <inheritdoc />
    public partial class AddProductsStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl_Back",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl_Back",
                table: "Products");
        }
    }
}
