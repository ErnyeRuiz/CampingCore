using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToCampSiteImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "CampSiteImages",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "CampSiteImages");
        }
    }
}
