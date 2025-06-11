using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeePortal.Migrations
{
    /// <inheritdoc />
    public partial class @fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Rename columns in Employees table for clarity and FK usage
        migrationBuilder.RenameColumn(
            name: "Type",
            table: "Employees",
            newName: "EmployeeTypeId");

        migrationBuilder.RenameColumn(
            name: "Department",
            table: "Employees",
            newName: "DepartmentId");

        // Removed addition of new TypeId column since EmployeeTypeId is the correct one

        // Create Departments table
        migrationBuilder.CreateTable(
            name: "Departments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Departments", x => x.Id);
            });

        // Create EmployeeTypes table
        migrationBuilder.CreateTable(
            name: "EmployeeTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmployeeTypes", x => x.Id);
            });

        // Seed data for Departments
        migrationBuilder.InsertData(
            table: "Departments",
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                    { 1, "IT" },
                    { 2, "HR" },
                    { 3, "Sales" },
                    { 4, "Admin" }
            });

        // Seed data for EmployeeTypes
        migrationBuilder.InsertData(
            table: "EmployeeTypes",
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                    { 1, "Permanent" },
                    { 2, "Temporary" },
                    { 3, "Contract" },
                    { 4, "Intern" }
            });

        // Create indexes for foreign keys
        migrationBuilder.CreateIndex(
            name: "IX_Employees_DepartmentId",
            table: "Employees",
            column: "DepartmentId");

        migrationBuilder.CreateIndex(
            name: "IX_Employees_EmployeeTypeId",
            table: "Employees",
            column: "EmployeeTypeId");

        // Add foreign key constraints with cascade on delete for Department,
        // restrict delete for EmployeeType to avoid accidental removal
        migrationBuilder.AddForeignKey(
            name: "FK_Employees_Departments_DepartmentId",
            table: "Employees",
            column: "DepartmentId",
            principalTable: "Departments",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Employees_EmployeeTypes_EmployeeTypeId",
            table: "Employees",
            column: "EmployeeTypeId",
            principalTable: "EmployeeTypes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Remove foreign keys
        migrationBuilder.DropForeignKey(
            name: "FK_Employees_Departments_DepartmentId",
            table: "Employees");

        migrationBuilder.DropForeignKey(
            name: "FK_Employees_EmployeeTypes_EmployeeTypeId",
            table: "Employees");

        // Drop the Departments and EmployeeTypes tables
        migrationBuilder.DropTable(
            name: "Departments");

        migrationBuilder.DropTable(
            name: "EmployeeTypes");

        // Drop indexes
        migrationBuilder.DropIndex(
            name: "IX_Employees_DepartmentId",
            table: "Employees");

        migrationBuilder.DropIndex(
            name: "IX_Employees_EmployeeTypeId",
            table: "Employees");

        // Rename columns back to original names
        migrationBuilder.RenameColumn(
            name: "EmployeeTypeId",
            table: "Employees",
            newName: "Type");

        migrationBuilder.RenameColumn(
            name: "DepartmentId",
            table: "Employees",
            newName: "Department");
    }
}
}
