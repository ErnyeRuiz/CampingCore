using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRatingColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "CampSites",
                type: "DECIMAL(4,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "CampSites",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(4,2)",
                oldDefaultValue: 0m);
        }
    }
}
