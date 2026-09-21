using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomaticCollegeTimetableGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddClassNameToSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "Subjects");
        }
    }
}
