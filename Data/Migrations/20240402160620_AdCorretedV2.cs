using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomFinder4You.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdCorretedV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.AddColumn<byte[]>(
                name: "MainPhoto",
                table: "Ads",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoFormat",
                table: "Ads",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
   migrationBuilder.DropColumn(
                name: "MainPhoto",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "PhotoFormat",
                table: "Ads");
        }
    }
}
