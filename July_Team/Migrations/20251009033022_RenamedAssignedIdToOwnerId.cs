using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace July_Team.Migrations
{
    /// <inheritdoc />
    public partial class RenamedAssignedIdToOwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssignedToId",
                table: "Tasks",
                newName: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Tasks",
                newName: "AssignedToId");
        }
    }
}
