using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveStationJanitor.Core.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStationIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_stations_id",
                table: "Stations",
                column: "id",
                unique: true,
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_stations_id",
                table: "Stations");
        }
    }
}
