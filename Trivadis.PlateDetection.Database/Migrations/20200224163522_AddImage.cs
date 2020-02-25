using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trivadis.PlateDetection.Database.Migrations
{
    public partial class AddImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "imagedata",
                table: "Jobs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imagedata",
                table: "Jobs");
        }
    }
}
