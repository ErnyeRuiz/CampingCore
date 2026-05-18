using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampingCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveImageBase64AddImageUrlNotNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageBase64",
                table: "CampSiteImages");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "CampSiteImages",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2083)",
                oldMaxLength: 2083,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "CampSiteImages",
                type: "nvarchar(2083)",
                maxLength: 2083,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2083)",
                oldMaxLength: 2083);

            migrationBuilder.AddColumn<string>(
                name: "ImageBase64",
                table: "CampSiteImages",
                type: "nvarchar(MAX)",
                nullable: false,
                defaultValue: "");
        }
    }
}
