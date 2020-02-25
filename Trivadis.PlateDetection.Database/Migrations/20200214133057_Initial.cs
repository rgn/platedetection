using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trivadis.PlateDetection.Database.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    jobid = table.Column<Guid>(nullable: false),
                    filename = table.Column<string>(nullable: true),
                    imageheight = table.Column<int>(nullable: false),
                    imagewidth = table.Column<int>(nullable: false),
                    totalprocessingtimeinms = table.Column<float>(nullable: false),
                    result = table.Column<string>(nullable: true),
                    state = table.Column<int>(nullable: false),
                    createdat = table.Column<DateTime>(nullable: false),
                    createdby = table.Column<string>(nullable: true),
                    lastupdatedat = table.Column<DateTime>(nullable: false),
                    lastupdatedby = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jobs", x => x.jobid);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    detectionresultid = table.Column<Guid>(nullable: false),
                    processingtimeinms = table.Column<float>(nullable: false),
                    region = table.Column<string>(nullable: true),
                    regionconfidence = table.Column<int>(nullable: false),
                    requestedtopn = table.Column<int>(nullable: false),
                    jobid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_results", x => x.detectionresultid);
                    table.ForeignKey(
                        name: "fk_results_jobs_jobid",
                        column: x => x.jobid,
                        principalTable: "Jobs",
                        principalColumn: "jobid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plates",
                columns: table => new
                {
                    detectedplateid = table.Column<Guid>(nullable: false),
                    matchestemplate = table.Column<bool>(nullable: false),
                    overallconfidence = table.Column<float>(nullable: false),
                    characters = table.Column<string>(nullable: true),
                    detectionresultid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plates", x => x.detectedplateid);
                    table.ForeignKey(
                        name: "fk_plates_results_detectionresultid",
                        column: x => x.detectionresultid,
                        principalTable: "Results",
                        principalColumn: "detectionresultid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_plates_detectionresultid",
                table: "Plates",
                column: "detectionresultid");

            migrationBuilder.CreateIndex(
                name: "ix_results_jobid",
                table: "Results",
                column: "jobid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plates");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
