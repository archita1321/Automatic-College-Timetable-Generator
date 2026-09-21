using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomaticCollegeTimetableGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddClassNameToClassoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "Classrooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "Classrooms");
        }
    }
}
