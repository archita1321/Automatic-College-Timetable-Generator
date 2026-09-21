using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomaticCollegeTimetableGenerator.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubjectType",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectType",
                table: "Subjects");
        }
    }
}
