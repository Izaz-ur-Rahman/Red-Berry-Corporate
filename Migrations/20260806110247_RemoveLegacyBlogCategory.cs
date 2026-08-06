using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedBerryCorporate.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLegacyBlogCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Blogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Blogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
