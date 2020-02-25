using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trivadis.PlateDetection.Database.Migrations
{
    public partial class AddRectangles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rectangle",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    x = table.Column<int>(nullable: false),
                    y = table.Column<int>(nullable: false),
                    width = table.Column<int>(nullable: false),
                    height = table.Column<int>(nullable: false),
                    jobid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rectangle", x => x.id);
                    table.ForeignKey(
                        name: "fk_rectangle_jobs_jobid",
                        column: x => x.jobid,
                        principalTable: "Jobs",
                        principalColumn: "jobid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_rectangle_jobid",
                table: "rectangle",
                column: "jobid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rectangle");
        }
    }
}
