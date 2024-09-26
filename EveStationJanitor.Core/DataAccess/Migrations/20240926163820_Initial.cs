using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveStationJanitor.Core.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityTags",
                columns: table => new
                {
                    key = table.Column<string>(type: "TEXT", nullable: false),
                    tag = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity_tags", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "ItemCategories",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MapRegions",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_map_regions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ItemGroups",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    category_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_item_groups_item_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "ItemCategories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MapSolarSystems",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    region_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_map_solar_systems", x => x.id);
                    table.ForeignKey(
                        name: "fk_map_solar_systems_map_regions_region_id",
                        column: x => x.region_id,
                        principalTable: "MapRegions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    group_id = table.Column<int>(type: "INTEGER", nullable: false),
                    volume = table.Column<float>(type: "REAL", nullable: false),
                    mass = table.Column<float>(type: "REAL", nullable: false),
                    portion_size = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_types", x => x.id);
                    table.ForeignKey(
                        name: "fk_item_types_item_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "ItemGroups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false),
                    solar_system_id = table.Column<int>(type: "INTEGER", nullable: false),
                    owner_corporation_id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    reprocessing_efficiency = table.Column<double>(type: "REAL", nullable: false),
                    reprocessing_tax = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stations", x => x.id);
                    table.ForeignKey(
                        name: "fk_stations_map_solar_systems_solar_system_id",
                        column: x => x.solar_system_id,
                        principalTable: "MapSolarSystems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemTypeMaterials",
                columns: table => new
                {
                    item_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    material_item_type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    quantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_type_materials", x => new { x.item_type_id, x.material_item_type_id });
                    table.ForeignKey(
                        name: "fk_item_type_materials_item_types_item_type_id",
                        column: x => x.item_type_id,
                        principalTable: "ItemTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_item_type_materials_item_types_material_item_type_id",
                        column: x => x.material_item_type_id,
                        principalTable: "ItemTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MarketOrders",
                columns: table => new
                {
                    order_id = table.Column<long>(type: "INTEGER", nullable: false),
                    duration = table.Column<int>(type: "INTEGER", nullable: false),
                    is_buy_order = table.Column<bool>(type: "INTEGER", nullable: false),
                    issued = table.Column<string>(type: "TEXT", nullable: false),
                    location_id = table.Column<long>(type: "INTEGER", nullable: false),
                    min_volume = table.Column<int>(type: "INTEGER", nullable: false),
                    price = table.Column<double>(type: "REAL", nullable: false),
                    range = table.Column<int>(type: "INTEGER", nullable: false),
                    system_id = table.Column<int>(type: "INTEGER", nullable: false),
                    type_id = table.Column<int>(type: "INTEGER", nullable: false),
                    volume_remaining = table.Column<int>(type: "INTEGER", nullable: false),
                    volume_total = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_market_orders", x => x.order_id);
                    table.ForeignKey(
                        name: "fk_market_orders_item_types_type_id",
                        column: x => x.type_id,
                        principalTable: "ItemTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_market_orders_map_solar_systems_system_id",
                        column: x => x.system_id,
                        principalTable: "MapSolarSystems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_market_orders_stations_location_id",
                        column: x => x.location_id,
                        principalTable: "Stations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_item_groups_category_id",
                table: "ItemGroups",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_type_materials_material_item_type_id",
                table: "ItemTypeMaterials",
                column: "material_item_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_types_group_id",
                table: "ItemTypes",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_map_solar_systems_region_id",
                table: "MapSolarSystems",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_market_orders_location_id",
                table: "MarketOrders",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_market_orders_system_id",
                table: "MarketOrders",
                column: "system_id");

            migrationBuilder.CreateIndex(
                name: "ix_market_orders_type_id",
                table: "MarketOrders",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_stations_solar_system_id",
                table: "Stations",
                column: "solar_system_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityTags");

            migrationBuilder.DropTable(
                name: "ItemTypeMaterials");

            migrationBuilder.DropTable(
                name: "MarketOrders");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "ItemGroups");

            migrationBuilder.DropTable(
                name: "MapSolarSystems");

            migrationBuilder.DropTable(
                name: "ItemCategories");

            migrationBuilder.DropTable(
                name: "MapRegions");
        }
    }
}
