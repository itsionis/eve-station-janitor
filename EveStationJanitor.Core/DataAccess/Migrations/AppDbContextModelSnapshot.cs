﻿// <auto-generated />
using System;
using EveStationJanitor.Core.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EveStationJanitor.Core.DataAccess.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int?>("AllianceId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("alliance_id");

                    b.Property<string>("CharacterOwnerHash")
                        .IsRequired()
                        .HasMaxLength(28)
                        .HasColumnType("TEXT")
                        .HasColumnName("character_owner_hash");

                    b.Property<int>("CorporationId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("corporation_id");

                    b.Property<int>("EveCharacterId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("eve_character_id");

                    b.Property<int?>("FactionId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("faction_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_characters");

                    b.HasIndex("EveCharacterId")
                        .IsUnique()
                        .HasDatabaseName("ix_characters_eve_character_id");

                    b.ToTable("Characters", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.CharacterAuthToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("CharacterId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("character_id");

                    b.Property<byte[]>("EncryptedRefreshToken")
                        .IsRequired()
                        .HasColumnType("BLOB")
                        .HasColumnName("encrypted_refresh_token");

                    b.Property<string>("ExpiresOn")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("expires_on");

                    b.HasKey("Id")
                        .HasName("pk_character_auth_tokens");

                    b.HasIndex("CharacterId")
                        .HasDatabaseName("ix_character_auth_tokens_character_id");

                    b.ToTable("CharacterAuthTokens", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.CharacterAuthTokenScope", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("CharacterAuthTokenId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("character_auth_token_id");

                    b.Property<string>("Scope")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT")
                        .HasColumnName("scope");

                    b.HasKey("Id")
                        .HasName("pk_character_auth_token_scopes");

                    b.HasIndex("CharacterAuthTokenId")
                        .HasDatabaseName("ix_character_auth_token_scopes_character_auth_token_id");

                    b.ToTable("CharacterAuthTokenScopes", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.EntityTag", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(1024)
                        .HasColumnType("TEXT")
                        .HasColumnName("key");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("TEXT")
                        .HasColumnName("tag");

                    b.HasKey("Key")
                        .HasName("pk_entity_tags");

                    b.ToTable("EntityTags", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.ItemCategory", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_item_categories");

                    b.ToTable("ItemCategories", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.ItemGroup", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("CategoryId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("category_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_item_groups");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("ix_item_groups_category_id");

                    b.ToTable("ItemGroups", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.ItemType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("group_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<int>("PortionSize")
                        .HasColumnType("INTEGER")
                        .HasColumnName("portion_size");

                    b.HasKey("Id")
                        .HasName("pk_item_types");

                    b.HasIndex("GroupId")
                        .HasDatabaseName("ix_item_types_group_id");

                    b.ToTable("ItemTypes", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.ItemTypeMaterial", b =>
                {
                    b.Property<int>("ItemTypeId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("item_type_id");

                    b.Property<int>("MaterialItemTypeId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("material_item_type_id");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER")
                        .HasColumnName("quantity");

                    b.HasKey("ItemTypeId", "MaterialItemTypeId")
                        .HasName("pk_item_type_materials");

                    b.HasIndex("MaterialItemTypeId")
                        .HasDatabaseName("ix_item_type_materials_material_item_type_id");

                    b.ToTable("ItemTypeMaterials", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.MapRegion", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_map_regions");

                    b.ToTable("MapRegions", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.MapSolarSystem", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<int>("RegionId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("region_id");

                    b.HasKey("Id")
                        .HasName("pk_map_solar_systems");

                    b.HasIndex("RegionId")
                        .HasDatabaseName("ix_map_solar_systems_region_id");

                    b.ToTable("MapSolarSystems", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.MarketOrder", b =>
                {
                    b.Property<long>("OrderId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("order_id");

                    b.Property<int>("Duration")
                        .HasColumnType("INTEGER")
                        .HasColumnName("duration");

                    b.Property<bool>("IsBuyOrder")
                        .HasColumnType("INTEGER")
                        .HasColumnName("is_buy_order");

                    b.Property<string>("Issued")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("issued");

                    b.Property<long>("LocationId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("location_id");

                    b.Property<long>("MinVolume")
                        .HasColumnType("INTEGER")
                        .HasColumnName("min_volume");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT")
                        .HasColumnName("price");

                    b.Property<int>("Range")
                        .HasColumnType("INTEGER")
                        .HasColumnName("range");

                    b.Property<int>("SystemId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("system_id");

                    b.Property<int>("TypeId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("type_id");

                    b.Property<long>("VolumeRemaining")
                        .HasColumnType("INTEGER")
                        .HasColumnName("volume_remaining");

                    b.Property<long>("VolumeTotal")
                        .HasColumnType("INTEGER")
                        .HasColumnName("volume_total");

                    b.HasKey("OrderId")
                        .HasName("pk_market_orders");

                    b.HasIndex("LocationId")
                        .HasDatabaseName("ix_market_orders_location_id");

                    b.HasIndex("SystemId")
                        .HasDatabaseName("ix_market_orders_system_id");

                    b.HasIndex("TypeId")
                        .HasDatabaseName("ix_market_orders_type_id");

                    b.ToTable("MarketOrders", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.Station", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<int>("OwnerCorporationId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("owner_corporation_id");

                    b.Property<decimal>("ReprocessingEfficiency")
                        .HasColumnType("TEXT")
                        .HasColumnName("reprocessing_efficiency");

                    b.Property<decimal>("ReprocessingTax")
                        .HasColumnType("TEXT")
                        .HasColumnName("reprocessing_tax");

                    b.Property<int>("SolarSystemId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("solar_system_id");

                    b.HasKey("Id")
                        .HasName("pk_stations");

                    b.HasIndex("Id")
                        .IsUnique()
                        .IsDescending()
                        .HasDatabaseName("ix_stations_id");

                    b.HasIndex("SolarSystemId")
                        .HasDatabaseName("ix_stations_solar_system_id");

                    b.ToTable("Stations", (string)null);
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.CharacterAuthToken", b =>
                {
                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.Character", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_character_auth_tokens_characters_character_id");

                    b.Navigation("Character");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.CharacterAuthTokenScope", b =>
                {
                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.CharacterAuthToken", "CharacterAuthToken")
                        .WithMany("Scopes")
                        .HasForeignKey("CharacterAuthTokenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_character_auth_token_scopes_character_auth_tokens_character_auth_token_id");

                    b.Navigation("CharacterAuthToken");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.ItemGroup", b =>
                {
                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.ItemCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_item_groups_item_categories_category_id");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.ItemType", b =>
                {
                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.ItemGroup", "Group")
                        .WithMany("ItemTypes")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_item_types_item_groups_group_id");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.ItemTypeMaterial", b =>
                {
                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.ItemType", "ItemType")
                        .WithMany("Materials")
                        .HasForeignKey("ItemTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_item_type_materials_item_types_item_type_id");

                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.ItemType", "MaterialType")
                        .WithMany()
                        .HasForeignKey("MaterialItemTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_item_type_materials_item_types_material_item_type_id");

                    b.Navigation("ItemType");

                    b.Navigation("MaterialType");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.MapSolarSystem", b =>
                {
                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.MapRegion", "Region")
                        .WithMany("Systems")
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_map_solar_systems_map_regions_region_id");

                    b.Navigation("Region");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.MarketOrder", b =>
                {
                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.Station", "Station")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_market_orders_stations_location_id");

                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.MapSolarSystem", "SolarSystem")
                        .WithMany()
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_market_orders_map_solar_systems_system_id");

                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.ItemType", "ItemType")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_market_orders_item_types_type_id");

                    b.Navigation("ItemType");

                    b.Navigation("SolarSystem");

                    b.Navigation("Station");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.Station", b =>
                {
                    b.HasOne("EveStationJanitor.Core.DataAccess.Entities.MapSolarSystem", "SolarSystem")
                        .WithMany("Stations")
                        .HasForeignKey("SolarSystemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_stations_map_solar_systems_solar_system_id");

                    b.Navigation("SolarSystem");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.CharacterAuthToken", b =>
                {
                    b.Navigation("Scopes");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.ItemGroup", b =>
                {
                    b.Navigation("ItemTypes");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.ItemType", b =>
                {
                    b.Navigation("Materials");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.MapRegion", b =>
                {
                    b.Navigation("Systems");
                });

            modelBuilder.Entity("EveStationJanitor.Core.DataAccess.Entities.MapSolarSystem", b =>
                {
                    b.Navigation("Stations");
                });
#pragma warning restore 612, 618
        }
    }
}
