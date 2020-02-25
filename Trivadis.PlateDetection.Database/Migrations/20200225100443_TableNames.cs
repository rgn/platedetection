using Microsoft.EntityFrameworkCore.Migrations;

namespace Trivadis.PlateDetection.Database.Migrations
{
    public partial class TableNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rectangle_jobs_jobid",
                table: "rectangle");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rectangle",
                table: "rectangle");

            migrationBuilder.RenameTable(
                name: "Results",
                newName: "results");

            migrationBuilder.RenameTable(
                name: "Plates",
                newName: "plates");

            migrationBuilder.RenameTable(
                name: "Jobs",
                newName: "jobs");

            migrationBuilder.RenameTable(
                name: "rectangle",
                newName: "rectangles");

            migrationBuilder.RenameIndex(
                name: "ix_rectangle_jobid",
                table: "rectangles",
                newName: "ix_rectangles_jobid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rectangles",
                table: "rectangles",
                column: "rectangleid");

            migrationBuilder.AddForeignKey(
                name: "fk_rectangles_jobs_jobid",
                table: "rectangles",
                column: "jobid",
                principalTable: "jobs",
                principalColumn: "jobid",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rectangles_jobs_jobid",
                table: "rectangles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rectangles",
                table: "rectangles");

            migrationBuilder.RenameTable(
                name: "results",
                newName: "Results");

            migrationBuilder.RenameTable(
                name: "plates",
                newName: "Plates");

            migrationBuilder.RenameTable(
                name: "jobs",
                newName: "Jobs");

            migrationBuilder.RenameTable(
                name: "rectangles",
                newName: "rectangle");

            migrationBuilder.RenameIndex(
                name: "ix_rectangles_jobid",
                table: "rectangle",
                newName: "ix_rectangle_jobid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rectangle",
                table: "rectangle",
                column: "rectangleid");

            migrationBuilder.AddForeignKey(
                name: "fk_rectangle_jobs_jobid",
                table: "rectangle",
                column: "jobid",
                principalTable: "Jobs",
                principalColumn: "jobid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
