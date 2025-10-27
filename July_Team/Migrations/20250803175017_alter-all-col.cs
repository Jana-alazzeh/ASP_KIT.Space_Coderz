using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace July_Team.Migrations
{
    /// <inheritdoc />
    public partial class alterallcol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "city",
                table: "employees",
                newName: "Employee_country");

            migrationBuilder.RenameColumn(
                name: "Salary",
                table: "employees",
                newName: "Employee_Salary");

            migrationBuilder.RenameColumn(
                name: "Hire_date",
                table: "employees",
                newName: "Employee_Hire_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Employee_country",
                table: "employees",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Employee_Salary",
                table: "employees",
                newName: "Salary");

            migrationBuilder.RenameColumn(
                name: "Employee_Hire_date",
                table: "employees",
                newName: "Hire_date");
        }
    }
}
