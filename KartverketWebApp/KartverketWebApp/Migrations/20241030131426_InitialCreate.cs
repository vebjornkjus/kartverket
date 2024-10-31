using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KartverketWebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Bruker",
                columns: table => new
                {
                    BrukerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Brukernavn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Passord = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BrukerType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bruker", x => x.BrukerId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Koordinater",
                columns: table => new
                {
                    KoordinatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Rekkefolge = table.Column<int>(type: "int", nullable: false),
                    Nord = table.Column<double>(type: "double", nullable: false),
                    Ost = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Koordinater", x => x.KoordinatId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fornavn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Etternavn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefon = table.Column<int>(type: "int", nullable: false),
                    BrukerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_Person_Bruker_BrukerId",
                        column: x => x.BrukerId,
                        principalTable: "Bruker",
                        principalColumn: "BrukerId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Kart",
                columns: table => new
                {
                    KartEndringId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KoordinaterId = table.Column<int>(type: "int", nullable: true),
                    Koordsys = table.Column<int>(type: "int", nullable: false),
                    Tittel = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Beskrivelse = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MapType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RapportType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    KoordinaterKoordinatId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kart", x => x.KartEndringId);
                    table.ForeignKey(
                        name: "FK_Kart_Koordinater_KoordinaterId",
                        column: x => x.KoordinaterId,
                        principalTable: "Koordinater",
                        principalColumn: "KoordinatId");
                    table.ForeignKey(
                        name: "FK_Kart_Koordinater_KoordinaterKoordinatId",
                        column: x => x.KoordinaterKoordinatId,
                        principalTable: "Koordinater",
                        principalColumn: "KoordinatId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rapport",
                columns: table => new
                {
                    RapportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    KartEndringId = table.Column<int>(type: "int", nullable: false),
                    RapportStatus = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Opprettet = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rapport", x => x.RapportId);
                    table.ForeignKey(
                        name: "FK_Rapport_Kart_KartEndringId",
                        column: x => x.KartEndringId,
                        principalTable: "Kart",
                        principalColumn: "KartEndringId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rapport_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Kart_KoordinaterId",
                table: "Kart",
                column: "KoordinaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Kart_KoordinaterKoordinatId",
                table: "Kart",
                column: "KoordinaterKoordinatId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_BrukerId",
                table: "Person",
                column: "BrukerId");

            migrationBuilder.CreateIndex(
                name: "IX_Rapport_KartEndringId",
                table: "Rapport",
                column: "KartEndringId");

            migrationBuilder.CreateIndex(
                name: "IX_Rapport_PersonId",
                table: "Rapport",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rapport");

            migrationBuilder.DropTable(
                name: "Kart");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Koordinater");

            migrationBuilder.DropTable(
                name: "Bruker");
        }
    }
}
