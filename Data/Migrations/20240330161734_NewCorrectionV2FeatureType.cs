using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomFinder4You.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewCorrectionV2FeatureType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isMandatory",
                table: "FeatureTypes",
                type: "BOOL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
        }
    }
}
