using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveStationJanitor.Core.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMassAndVolumeColumnsFromItemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "mass",
                table: "ItemTypes");

            migrationBuilder.DropColumn(
                name: "volume",
                table: "ItemTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "mass",
                table: "ItemTypes",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "volume",
                table: "ItemTypes",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
