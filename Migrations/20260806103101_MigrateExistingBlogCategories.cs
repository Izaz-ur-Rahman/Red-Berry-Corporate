using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedBerryCorporate.Migrations
{
    /// <inheritdoc />
    public partial class MigrateExistingBlogCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    UPDATE Blogs
    SET CategoryId =
        CASE
            WHEN LOWER(LTRIM(RTRIM(Category))) IN (
                'business',
                'bussines',
                'craft',
                'field notes',
                'good',
                'published',
                'studio'
            )
                THEN 4

            WHEN LOWER(LTRIM(RTRIM(Category))) = 'finance'
                THEN 3

            WHEN LOWER(LTRIM(RTRIM(Category))) = 'electronics'
                THEN 2

            WHEN LOWER(LTRIM(RTRIM(Category))) IN (
                '.net',
                'acacs',
                'design',
                'it',
                'java',
                'jhjkhk',
                'nm',
                't',
                'tech',
                'technology',
                'testing',
                'the system',
                'ttt',
                'ttwww',
                'v',
                'web',
                'wwww'
            )
                THEN 1

            ELSE CategoryId
        END
    WHERE Category IS NOT NULL;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
