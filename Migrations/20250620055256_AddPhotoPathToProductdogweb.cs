using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dogwebMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoPathToProductdogweb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Productsbydogweb",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Productsbydogweb");
        }
    }
}
