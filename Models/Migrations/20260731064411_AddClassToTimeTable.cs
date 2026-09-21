using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomaticCollegeTimetableGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddClassToTimeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "Timetables",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "Timetables");
        }
    }
}
