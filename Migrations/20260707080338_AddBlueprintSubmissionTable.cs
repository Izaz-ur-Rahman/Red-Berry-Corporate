using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedBerryCorporate.Migrations
{
    /// <inheritdoc />
    public partial class AddBlueprintSubmissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlueprintSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Whatsapp = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BusinessStage = table.Column<int>(type: "int", nullable: false),
                    ReviewMethod = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ambition = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    CorporateScore = table.Column<int>(type: "int", nullable: false),
                    FinancialScore = table.Column<int>(type: "int", nullable: false),
                    MarketScore = table.Column<int>(type: "int", nullable: false),
                    LegacyScore = table.Column<int>(type: "int", nullable: false),
                    StrongestLayer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExposedLayer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImprovementLayers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommendedPathway = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    OverallStatus = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PatternsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlueprintSubmissions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlueprintSubmissions");
        }
    }
}
