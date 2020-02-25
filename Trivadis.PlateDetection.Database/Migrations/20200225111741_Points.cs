using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trivadis.PlateDetection.Database.Migrations
{
    public partial class Points : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "result_points",
                columns: table => new
                {
                    pointid = table.Column<Guid>(nullable: false),
                    detectionresultid = table.Column<Guid>(nullable: false),
                    x = table.Column<int>(nullable: false),
                    y = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_result_points", x => x.pointid);
                    table.ForeignKey(
                        name: "fk_result_points_results_detectionresultid",
                        column: x => x.detectionresultid,
                        principalTable: "results",
                        principalColumn: "detectionresultid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_result_points_detectionresultid",
                table: "result_points",
                column: "detectionresultid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "result_points");
        }
    }
}
