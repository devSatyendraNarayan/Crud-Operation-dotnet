using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication1.Migrations
{
    public partial class s : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_student_tbl_departments_DepartmentsID",
                table: "tbl_student");

            migrationBuilder.DropIndex(
                name: "IX_tbl_student_DepartmentsID",
                table: "tbl_student");

            migrationBuilder.DropColumn(
                name: "DepartmentsID",
                table: "tbl_student");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentsID",
                table: "tbl_student",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_student_DepartmentsID",
                table: "tbl_student",
                column: "DepartmentsID");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_student_tbl_departments_DepartmentsID",
                table: "tbl_student",
                column: "DepartmentsID",
                principalTable: "tbl_departments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
