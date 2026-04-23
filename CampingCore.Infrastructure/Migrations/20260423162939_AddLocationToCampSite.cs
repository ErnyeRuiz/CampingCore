using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToCampSite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DireccionExacta",
                table: "CampSites",
                type: "NVARCHAR(500)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdCanton",
                table: "CampSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdDistrito",
                table: "CampSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdProvincia",
                table: "CampSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "CampSites",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DireccionExacta",
                table: "CampSites");

            migrationBuilder.DropColumn(
                name: "IdCanton",
                table: "CampSites");

            migrationBuilder.DropColumn(
                name: "IdDistrito",
                table: "CampSites");

            migrationBuilder.DropColumn(
                name: "IdProvincia",
                table: "CampSites");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "CampSites");
        }
    }
}
