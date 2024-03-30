using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomFinder4You.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorrectFeatureType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ads_AspNetUsers_ApplicationUserId",
                table: "Ads");

            migrationBuilder.DropIndex(
                name: "IX_Ads_ApplicationUserId",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Ads");

            migrationBuilder.CreateIndex(
                name: "IX_Ads_UserID",
                table: "Ads",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ads_AspNetUsers_UserID",
                table: "Ads",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ads_AspNetUsers_UserID",
                table: "Ads");

            migrationBuilder.DropIndex(
                name: "IX_Ads_UserID",
                table: "Ads");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Ads",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ads_ApplicationUserId",
                table: "Ads",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ads_AspNetUsers_ApplicationUserId",
                table: "Ads",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
