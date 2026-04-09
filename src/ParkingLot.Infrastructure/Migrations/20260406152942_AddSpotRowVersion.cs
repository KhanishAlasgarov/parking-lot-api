using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingLot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpotRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "ParkingSpots",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ParkingSpots");
        }
    }
}
