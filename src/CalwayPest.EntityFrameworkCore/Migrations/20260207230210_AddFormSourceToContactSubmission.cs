using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalwayPest.Migrations
{
    /// <inheritdoc />
    public partial class AddFormSourceToContactSubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormSource",
                table: "AppContactSubmissions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "ContactForm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormSource",
                table: "AppContactSubmissions");
        }
    }
}
