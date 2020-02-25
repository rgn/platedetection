using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trivadis.PlateDetection.Database.Migrations
{
    public partial class RectangleID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_rectangle",
                table: "rectangle");

            migrationBuilder.DropColumn(
                name: "id",
                table: "rectangle");

            migrationBuilder.AddColumn<Guid>(
                name: "rectangleid",
                table: "rectangle",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_rectangle",
                table: "rectangle",
                column: "rectangleid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_rectangle",
                table: "rectangle");

            migrationBuilder.DropColumn(
                name: "rectangleid",
                table: "rectangle");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "rectangle",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_rectangle",
                table: "rectangle",
                column: "id");
        }
    }
}
