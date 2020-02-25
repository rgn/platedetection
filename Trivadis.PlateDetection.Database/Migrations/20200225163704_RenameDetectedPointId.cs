using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trivadis.PlateDetection.Database.Migrations
{
    public partial class RenameDetectedPointId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_result_points",
                table: "result_points");

            migrationBuilder.DropColumn(
                name: "pointid",
                table: "result_points");

            migrationBuilder.AddColumn<Guid>(
                name: "detectedpointid",
                table: "result_points",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_result_points",
                table: "result_points",
                column: "detectedpointid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_result_points",
                table: "result_points");

            migrationBuilder.DropColumn(
                name: "detectedpointid",
                table: "result_points");

            migrationBuilder.AddColumn<Guid>(
                name: "pointid",
                table: "result_points",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_result_points",
                table: "result_points",
                column: "pointid");
        }
    }
}
