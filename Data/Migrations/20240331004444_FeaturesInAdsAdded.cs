using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomFinder4You.Data.Migrations
{
    /// <inheritdoc />
    public partial class FeaturesInAdsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdId",
                table: "Features",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Features_AdId",
                table: "Features",
                column: "AdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Ads_AdId",
                table: "Features",
                column: "AdId",
                principalTable: "Ads",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Features_Ads_AdId",
                table: "Features");

            migrationBuilder.DropIndex(
                name: "IX_Features_AdId",
                table: "Features");

            migrationBuilder.DropColumn(
                name: "AdId",
                table: "Features");
        }
    }
}
