using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP2084_Project_Eventour.Migrations
{
    public partial class addedphotostartandendtimetoeventdetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "eventDate",
                table: "EventDetails",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "EventDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "EventDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "EventDetails");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "EventDetails");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "EventDetails",
                newName: "eventDate");
        }
    }
}
