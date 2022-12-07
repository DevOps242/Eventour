using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace COMP2084_Project_Eventour.Migrations
{
    public partial class removephotoformdetailmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "EventDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "EventDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
