using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedBerryCorporate.Migrations
{
    /// <inheritdoc />
    public partial class MigrateBlogCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    UPDATE Blogs
    SET CategoryId = BlogCategories.Id
    FROM Blogs
    INNER JOIN BlogCategories
        ON LOWER(LTRIM(RTRIM(Blogs.Category))) =
           LOWER(LTRIM(RTRIM(BlogCategories.Name)))
    WHERE Blogs.CategoryId IS NULL
      AND Blogs.Category IS NOT NULL;
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
