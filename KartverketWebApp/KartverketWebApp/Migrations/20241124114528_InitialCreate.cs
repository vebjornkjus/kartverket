using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Bruker",
                columns: table => new
                {
                    BrukerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "longtext", nullable: false)
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
                name: "Steddata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fylkenavn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Kommunenavn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fylkenummer = table.Column<int>(type: "int", nullable: false),
                    Kommunenummer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steddata", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginProvider = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    BrukerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.PersonId);
                    table.ForeignKey(
                        name: "FK_Person_Bruker_BrukerId",
                        column: x => x.BrukerId,
                        principalTable: "Bruker",
                        principalColumn: "BrukerId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Kart",
                columns: table => new
                {
                    KartEndringId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tittel = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Beskrivelse = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FilePath = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Koordsys = table.Column<int>(type: "int", nullable: false),
                    SteddataId = table.Column<int>(type: "int", nullable: true),
                    MapType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RapportType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kart", x => x.KartEndringId);
                    table.ForeignKey(
                        name: "FK_Kart_Steddata_SteddataId",
                        column: x => x.SteddataId,
                        principalTable: "Steddata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ansatt",
                columns: table => new
                {
                    AnsattId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Kommunenummer = table.Column<int>(type: "int", nullable: false),
                    AnsettelsesDato = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ansatt", x => x.AnsattId);
                    table.ForeignKey(
                        name: "FK_Ansatt_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
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
                    Ost = table.Column<double>(type: "double", nullable: false),
                    KartEndringId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Koordinater", x => x.KoordinatId);
                    table.ForeignKey(
                        name: "FK_Koordinater_Kart_KartEndringId",
                        column: x => x.KartEndringId,
                        principalTable: "Kart",
                        principalColumn: "KartEndringId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Rapport",
                columns: table => new
                {
                    RapportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RapportStatus = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Opprettet = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TildelAnsattId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    KartEndringId = table.Column<int>(type: "int", nullable: false),
                    BehandletDato = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rapport", x => x.RapportId);
                    table.ForeignKey(
                        name: "FK_Rapport_Ansatt_TildelAnsattId",
                        column: x => x.TildelAnsattId,
                        principalTable: "Ansatt",
                        principalColumn: "AnsattId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rapport_Kart_KartEndringId",
                        column: x => x.KartEndringId,
                        principalTable: "Kart",
                        principalColumn: "KartEndringId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rapport_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Meldinger",
                columns: table => new
                {
                    MeldingsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RapportId = table.Column<int>(type: "int", nullable: false),
                    SenderPersonId = table.Column<int>(type: "int", nullable: false),
                    MottakerPersonId = table.Column<int>(type: "int", nullable: false),
                    Innhold = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tidsstempel = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meldinger", x => x.MeldingsId);
                    table.ForeignKey(
                        name: "FK_Meldinger_Person_MottakerPersonId",
                        column: x => x.MottakerPersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Meldinger_Person_SenderPersonId",
                        column: x => x.SenderPersonId,
                        principalTable: "Person",
                        principalColumn: "PersonId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Meldinger_Rapport_RapportId",
                        column: x => x.RapportId,
                        principalTable: "Rapport",
                        principalColumn: "RapportId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Bruker",
                columns: new[] { "BrukerId", "BrukerType", "Email", "Passord" },
                values: new object[,]
                {
                    { 1, "standard", "bruker1@example.com", "passord123" },
                    { 2, "standard", "bruker2@example.com", "passord123" },
                    { 3, "standard", "bruker3@example.com", "passord123" },
                    { 4, "standard", "bruker4@example.com", "passord123" },
                    { 5, "standard", "bruker5@example.com", "passord123" },
                    { 6, "standard", "bruker6@example.com", "passord123" },
                    { 7, "standard", "bruker7@example.com", "passord123" },
                    { 8, "standard", "bruker8@example.com", "passord123" },
                    { 9, "standard", "bruker9@example.com", "passord123" },
                    { 10, "standard", "bruker10@example.com", "passord123" },
                    { 11, "spesial", "ambulansen@test.com", "passord123" },
                    { 12, "saksbehandler", "saksbehandler@example.com", "passord123" },
                    { 13, "admin", "admin@example.com", "passord123" },
                    { 14, "saksbehandler", "OsloOslo@example.com", "passord123" },
                    { 15, "saksbehandler", "VestlandBergen@example.com", "passord123" },
                    { 16, "saksbehandler", "RogalandStavanger@example.com", "passord123" },
                    { 17, "saksbehandler", "Tr�ndelagTrondheim@example.com", "passord123" },
                    { 18, "saksbehandler", "VikenDrammen@example.com", "passord123" },
                    { 19, "saksbehandler", "VikenFredrikstad@example.com", "passord123" },
                    { 20, "saksbehandler", "AgderKristiansand@example.com", "passord123" },
                    { 21, "saksbehandler", "RogalandSandnes@example.com", "passord123" },
                    { 22, "saksbehandler", "TromsOgFinnmarkTroms�@example.com", "passord123" },
                    { 23, "saksbehandler", "NordlandBod�@example.com", "passord123" }
                });

            migrationBuilder.InsertData(
                table: "Steddata",
                columns: new[] { "Id", "Fylkenavn", "Fylkenummer", "Kommunenavn", "Kommunenummer" },
                values: new object[,]
                {
                    { 1, "Oslo", 3, "Oslo", 301 },
                    { 2, "Vestland", 46, "Bergen", 4601 },
                    { 3, "Tr�ndelag", 50, "Trondheim", 5001 },
                    { 4, "Agder", 42, "Kristiansand", 4204 },
                    { 5, "Innlandet", 34, "V�g�", 3435 },
                    { 6, "Troms og Finnmark", 55, "Troms�", 5401 },
                    { 7, "Nordland", 18, "Bod�", 1804 },
                    { 8, "Oslo", 3, "Oslo", 301 },
                    { 9, "Finnmark", 56, "Vads�", 5405 },
                    { 10, "Vestfold og Telemark", 40, "Skien", 3807 }
                });

            migrationBuilder.InsertData(
                table: "Kart",
                columns: new[] { "KartEndringId", "Beskrivelse", "FilePath", "Koordsys", "MapType", "RapportType", "SteddataId", "Tittel" },
                values: new object[,]
                {
                    { 1, "Hull i veien på hovedgata. Dette hullet har blitt rapportert flere ganger og trenger umiddelbar oppmerksomhet fra vedlikeholdsteamet.", null, 4258, "Norge kart", "Veiskade", 1, "Veiskade" },
                    { 2, "Området ved elvebredden har vært oversvømt i flere dager, og vannstanden ser ut til å fortsette å stige.", null, 4258, "Norge kart", "Oversvømmelse", 2, "Oversvømt område" },
                    { 3, "Steinras som blokkerer en viktig sti i fjellområdet, og dette skaper en stor risiko for turgåere og fjellklatrere.", null, 4258, "Turkart", "Skredfare", 3, "Fjellskred" },
                    { 4, "Stien er kraftig overgrodd med planter, og det er tydelige tegn på erosjon langs hele strekningen.", null, 4258, "Turkart", "Stivedlikehold", 4, "Skadet sti" },
                    { 5, "Flere gamle båtvrak har samlet seg langs kystlinjen. Dette kan være farlig for mindre båter og svømmere.", null, 4258, "Sjøkart", "Ryddeaksjon", 5, "Båtvrak" },
                    { 6, "Alvorlig trafikkulykke på motorveien med flere kjøretøy involvert. Krever umiddelbar rydding for å unngå kø.", null, 4258, "Norge kart", "Trafikkulykke", 6, "Trafikkulykke" },
                    { 7, "Snøras i fjellområdet som har blokkert veien og kan utgjøre en fare for kommende trafikk.", null, 4258, "Turkart", "Snørasfare", 7, "Snøras" },
                    { 8, "Sykkelstien har store sprekker og hull som gjør det vanskelig for syklister å bruke den trygt.", null, 4258, "Norge kart", "Sykkelsti reparasjon", 8, "Sykkelsti skadet" },
                    { 9, "Båthavnen er overfylt med båter, noe som gjør det vanskelig for nye båter å legge til kai eller parkere.", null, 4258, "Sjøkart", "Overfylte båtplasser", 9, "Båthavn" },
                    { 10, "Fiskefeltet er overbeskattet, og det er behov for strengere regulering for å bevare fiskebestanden.", null, 4258, "Sjøkart", "Fiskeriforvaltning", 10, "Fiskefelt" }
                });

            migrationBuilder.InsertData(
                table: "Person",
                columns: new[] { "PersonId", "BrukerId", "Etternavn", "Fornavn" },
                values: new object[,]
                {
                    { 1, 1, "Nordmann", "Ola" },
                    { 2, 2, "Nordmann", "Kari" },
                    { 3, 3, "Hansen", "Per" },
                    { 4, 4, "Larsen", "Anne" },
                    { 5, 5, "Johansen", "Nina" },
                    { 6, 6, "Berg", "Erik" },
                    { 7, 7, "Olsen", "Lise" },
                    { 8, 8, "Moen", "Hans" },
                    { 9, 9, "Lie", "Mona" },
                    { 10, 10, "Halvorsen", "Tom" },
                    { 11, 11, "Lance", "Abu" },
                    { 12, 12, "Behandler", "Saks" },
                    { 13, 13, "Minh", "Adam" },
                    { 14, 14, "Oslo", "Ole" },
                    { 15, 15, "Bergen", "Bj�rn" },
                    { 16, 16, "Stavanger", "Siri" },
                    { 17, 17, "Trondheim", "Knut" },
                    { 18, 18, "Drammen", "Lena" },
                    { 19, 19, "Fredrikstad", "Marta" },
                    { 20, 20, "Kristiansand", "Nils" },
                    { 21, 21, "Sandnes", "Eva" },
                    { 22, 22, "Troms�", "Per" },
                    { 23, 23, "Bod�", "Ingrid" }
                });

            migrationBuilder.InsertData(
                table: "Ansatt",
                columns: new[] { "AnsattId", "AnsettelsesDato", "Kommunenummer", "PersonId" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 12 },
                    { 2, new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 301, 14 },
                    { 3, new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 4601, 15 },
                    { 4, new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1101, 16 },
                    { 5, new DateTime(2024, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 5001, 17 },
                    { 6, new DateTime(2024, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 3005, 18 },
                    { 7, new DateTime(2024, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 3004, 19 },
                    { 8, new DateTime(2024, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 4204, 20 },
                    { 9, new DateTime(2024, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1108, 21 },
                    { 10, new DateTime(2024, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 5401, 22 },
                    { 11, new DateTime(2024, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 1804, 23 }
                });

            migrationBuilder.InsertData(
                table: "Koordinater",
                columns: new[] { "KoordinatId", "KartEndringId", "Nord", "Ost", "Rekkefolge" },
                values: new object[,]
                {
                    { 1, 1, 59.913899999999998, 10.7522, 1 },
                    { 2, 1, 59.914000000000001, 10.7523, 2 },
                    { 3, 2, 60.391300000000001, 5.3220999999999998, 1 },
                    { 4, 2, 60.391500000000001, 5.3224, 2 },
                    { 5, 2, 60.391399999999997, 5.3223000000000003, 3 },
                    { 6, 3, 63.430500000000002, 10.395099999999999, 1 },
                    { 7, 3, 63.430599999999998, 10.395200000000001, 2 },
                    { 8, 3, 63.430700000000002, 10.395300000000001, 3 },
                    { 9, 3, 63.430799999999998, 10.3954, 4 },
                    { 10, 4, 58.146700000000003, 7.9946000000000002, 1 },
                    { 11, 4, 58.146799999999999, 7.9946999999999999, 2 },
                    { 12, 5, 61.8735, 9.0945999999999998, 1 },
                    { 13, 5, 61.873600000000003, 9.0946999999999996, 2 },
                    { 14, 5, 61.873699999999999, 9.0947999999999993, 3 },
                    { 15, 5, 61.873800000000003, 9.0949000000000009, 4 },
                    { 16, 5, 61.873899999999999, 9.0950000000000006, 5 },
                    { 17, 5, 61.874000000000002, 9.0951000000000004, 6 },
                    { 18, 6, 69.649199999999993, 18.955300000000001, 1 },
                    { 19, 6, 69.649299999999997, 18.955400000000001, 2 },
                    { 20, 6, 69.6494, 18.955500000000001, 3 },
                    { 21, 7, 68.4392, 17.427600000000002, 1 },
                    { 22, 7, 68.439300000000003, 17.427700000000002, 2 },
                    { 23, 8, 59.914099999999998, 10.7524, 1 },
                    { 24, 8, 59.914200000000001, 10.7525, 2 },
                    { 25, 9, 70.073800000000006, 29.749199999999998, 1 },
                    { 26, 9, 70.073899999999995, 29.749300000000002, 2 },
                    { 27, 9, 70.073999999999998, 29.749400000000001, 3 },
                    { 28, 9, 70.074100000000001, 29.749500000000001, 4 },
                    { 29, 10, 59.0578, 10.0364, 1 },
                    { 30, 10, 59.057899999999997, 10.0365, 2 }
                });

            migrationBuilder.InsertData(
                table: "Rapport",
                columns: new[] { "RapportId", "BehandletDato", "KartEndringId", "Opprettet", "PersonId", "RapportStatus", "TildelAnsattId" },
                values: new object[,]
                {
                    { 1, null, 1, new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7368), 1, "Uåpnet", 1 },
                    { 2, null, 2, new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7421), 2, "Under behandling", 1 },
                    { 3, new DateTime(2022, 3, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 3, new DateTime(2022, 3, 13, 10, 0, 0, 0, DateTimeKind.Unspecified), 3, "Avklart", 1 },
                    { 4, null, 4, new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7452), 4, "Uåpnet", 1 },
                    { 5, null, 5, new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7453), 5, "Under behandling", 1 },
                    { 6, new DateTime(2022, 8, 22, 14, 30, 0, 0, DateTimeKind.Unspecified), 6, new DateTime(2022, 8, 20, 14, 30, 0, 0, DateTimeKind.Unspecified), 6, "Avklart", 1 },
                    { 7, null, 7, new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7455), 7, "Uåpnet", 1 },
                    { 8, null, 8, new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7456), 8, "Under behandling", 1 },
                    { 9, new DateTime(2023, 1, 10, 9, 15, 0, 0, DateTimeKind.Unspecified), 9, new DateTime(2023, 1, 8, 9, 15, 0, 0, DateTimeKind.Unspecified), 9, "Avklart", 1 },
                    { 10, null, 10, new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7458), 10, "Uåpnet", 1 }
                });

            migrationBuilder.InsertData(
                table: "Meldinger",
                columns: new[] { "MeldingsId", "Innhold", "MottakerPersonId", "RapportId", "SenderPersonId", "Status", "Tidsstempel" },
                values: new object[,]
                {
                    { 1, "Hei Ola, vi trenger mer informasjon om veiskaden du rapporterte.", 1, 1, 12, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7522) },
                    { 2, "Hei, her er de detaljerte opplysningene om veiskaden.", 12, 1, 1, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7524) },
                    { 3, "Kari, kan du bekrefte oversv�mmelsen i omr�det du rapporterte?", 2, 2, 12, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7525) },
                    { 4, "Bekreftet, omr�det er fortsatt oversv�mt.", 12, 2, 2, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7526) },
                    { 5, "Per, vi trenger oppdateringer om fjellskredet.", 3, 3, 12, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7527) },
                    { 6, "Fjellskredet er n� under overv�king.", 12, 3, 3, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7528) },
                    { 7, "Anne, kan du sende bilder av den skadede stien?", 4, 4, 12, "sendt", new DateTime(2024, 11, 23, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7529) },
                    { 8, "Selvf�lgelig, her er bildene.", 12, 4, 4, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7532) },
                    { 9, "Nina, har du innsikt i b�tvrakene?", 5, 5, 12, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7533) },
                    { 10, "Ja, vi trenger assistanse for rydding.", 12, 5, 5, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7534) },
                    { 11, "Erik, vi har behov for rapporter om trafikkulykken.", 6, 6, 12, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7535) },
                    { 12, "Rapportene er under arbeid og vil bli levert snart.", 12, 6, 6, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7536) },
                    { 13, "Lise, sn�rasen er kritisk, kan du igangsette tiltak?", 7, 7, 12, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7537) },
                    { 14, "Tiltak er iverksatt for � h�ndtere sn�rasen.", 12, 7, 7, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7538) },
                    { 15, "Hans, vi trenger mer informasjon om sykkelsti-skaden.", 8, 8, 12, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7539) },
                    { 16, "Her er de n�dvendige detaljene.", 12, 8, 8, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7540) },
                    { 17, "Mona, kan du overv�ke fiskefeltet?", 9, 9, 12, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7541) },
                    { 18, "Fiskefeltet overv�kes kontinuerlig.", 12, 9, 9, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7542) },
                    { 19, "Tom, vi trenger data om fiskeriforvaltningen.", 10, 10, 12, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7543) },
                    { 20, "Dataene er samlet og kan sendes.", 12, 10, 10, "sendt", new DateTime(2024, 11, 24, 12, 45, 27, 793, DateTimeKind.Local).AddTicks(7544) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ansatt_PersonId",
                table: "Ansatt",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kart_SteddataId",
                table: "Kart",
                column: "SteddataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Koordinater_KartEndringId",
                table: "Koordinater",
                column: "KartEndringId");

            migrationBuilder.CreateIndex(
                name: "IX_Meldinger_MottakerPersonId",
                table: "Meldinger",
                column: "MottakerPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Meldinger_RapportId",
                table: "Meldinger",
                column: "RapportId");

            migrationBuilder.CreateIndex(
                name: "IX_Meldinger_SenderPersonId",
                table: "Meldinger",
                column: "SenderPersonId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Rapport_TildelAnsattId",
                table: "Rapport",
                column: "TildelAnsattId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Koordinater");

            migrationBuilder.DropTable(
                name: "Meldinger");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Rapport");

            migrationBuilder.DropTable(
                name: "Ansatt");

            migrationBuilder.DropTable(
                name: "Kart");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Steddata");

            migrationBuilder.DropTable(
                name: "Bruker");
        }
    }
}
