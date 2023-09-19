using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiyazDentalClinicAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSetWebPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetWebPage",
                table: "MyCases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SetWebPage",
                table: "MyCases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
