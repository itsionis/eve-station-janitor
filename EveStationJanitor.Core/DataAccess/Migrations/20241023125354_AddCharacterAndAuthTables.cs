using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EveStationJanitor.Core.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterAndAuthTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    eve_character_id = table.Column<int>(type: "INTEGER", nullable: false),
                    character_owner_hash = table.Column<string>(type: "TEXT", nullable: false),
                    alliance_id = table.Column<int>(type: "INTEGER", nullable: true),
                    faction_id = table.Column<int>(type: "INTEGER", nullable: true),
                    corporation_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterAuthTokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    encrypted_refresh_token = table.Column<byte[]>(type: "BLOB", nullable: false),
                    expires_on = table.Column<string>(type: "TEXT", nullable: false),
                    character_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_auth_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_character_auth_tokens_characters_character_id",
                        column: x => x.character_id,
                        principalTable: "Characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterAuthTokenScopes",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    scope = table.Column<string>(type: "TEXT", nullable: false),
                    character_auth_token_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_auth_token_scopes", x => x.id);
                    table.ForeignKey(
                        name: "fk_character_auth_token_scopes_character_auth_tokens_character_auth_token_id",
                        column: x => x.character_auth_token_id,
                        principalTable: "CharacterAuthTokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_character_auth_tokens_character_id",
                table: "CharacterAuthTokens",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "ix_character_auth_token_scopes_character_auth_token_id",
                table: "CharacterAuthTokenScopes",
                column: "character_auth_token_id");

            migrationBuilder.CreateIndex(
                name: "ix_characters_eve_character_id",
                table: "Characters",
                column: "eve_character_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterAuthTokenScopes");

            migrationBuilder.DropTable(
                name: "CharacterAuthTokens");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
