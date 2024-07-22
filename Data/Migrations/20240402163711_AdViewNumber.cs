using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomFinder4You.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdViewNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ViewNumber",
                table: "Ads",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewNumber",
                table: "Ads");
        }
    }
}
