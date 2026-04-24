using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameImageUrlToImageBase64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "CampSiteImages",
                newName: "ImageBase64");

            migrationBuilder.AlterColumn<string>(
                name: "ImageBase64",
                table: "CampSiteImages",
                type: "nvarchar(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2083)",
                oldMaxLength: 2083);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageBase64",
                table: "CampSiteImages",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)");

            migrationBuilder.RenameColumn(
                name: "ImageBase64",
                table: "CampSiteImages",
                newName: "ImageUrl");
        }
    }
}
