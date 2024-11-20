using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace KartverketWebApp.Data
{
    using System;
    using Microsoft.AspNetCore.Identity;

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Kart> Kart { get; set; }
        public DbSet<Koordinater> Koordinater { get; set; }
        public DbSet<Rapport> Rapport { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Bruker> Bruker { get; set; }
        public DbSet<Steddata> Steddata { get; set; } // Ny
        public DbSet<Ansatt> Ansatt { get; set; } // Ny
        public DbSet<Meldinger> Meldinger { get; set; } // Ny

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sikre at Identity-konfigurasjonen blir kj�rt f�rst
            base.OnModelCreating(modelBuilder);

            // Bruker konfigurasjoner
            modelBuilder.Entity<Bruker>()
                .HasMany(b => b.Personer)
                .WithOne(p => p.Bruker)
                .HasForeignKey(p => p.BrukerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Person konfigurasjoner
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Rapporter)
                .WithOne(r => r.Person)
                .HasForeignKey(r => r.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Kart konfigurasjoner
            modelBuilder.Entity<Kart>()
                .HasOne(k => k.Steddata)
                .WithOne(s => s.Kart)
                .HasForeignKey<Kart>(k => k.SteddataId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kart>()
                .HasMany(k => k.Koordinater)
                .WithOne(ko => ko.Kart)
                .HasForeignKey(ko => ko.KartEndringId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Kart>()
                .HasMany(k => k.Rapporter)
                .WithOne(r => r.Kart)
                .HasForeignKey(r => r.KartEndringId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ansatt konfigurasjoner
            modelBuilder.Entity<Ansatt>()
                .HasOne(a => a.Person)
                .WithMany(p => p.Ansatt) // Legg til ICollection<Ansatt> i Person-klassen hvis n�dvendig
                .HasForeignKey(a => a.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ansatt>()
                .HasMany(a => a.Rapporter)
                .WithOne(r => r.TildelAnsatt)
                .HasForeignKey(r => r.TildelAnsattId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rapport konfigurasjoner
            modelBuilder.Entity<Rapport>()
                .HasMany(r => r.Meldinger)
                .WithOne(m => m.Rapport)
                .HasForeignKey(m => m.RapportId)
                .OnDelete(DeleteBehavior.Restrict);

            // Meldinger konfigurasjoner
            modelBuilder.Entity<Meldinger>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Meldinger>()
                .HasOne(m => m.Mottaker)
                .WithMany()
                .HasForeignKey(m => m.MottakerPersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Steddata>().HasData(
                new { Id = 1, Fylkenavn = "Oslo", Kommunenavn = "Oslo", Fylkenummer = 3, Kommunenummer = 0301 },
                new { Id = 2, Fylkenavn = "Vestland", Kommunenavn = "Bergen", Fylkenummer = 46, Kommunenummer = 4601 },
                new { Id = 3, Fylkenavn = "Tr�ndelag", Kommunenavn = "Trondheim", Fylkenummer = 50, Kommunenummer = 5001 },
                new { Id = 4, Fylkenavn = "Agder", Kommunenavn = "Kristiansand", Fylkenummer = 42, Kommunenummer = 4204 },
                new { Id = 5, Fylkenavn = "Innlandet", Kommunenavn = "V�g�", Fylkenummer = 34, Kommunenummer = 3435 },
                new { Id = 6, Fylkenavn = "Troms og Finnmark", Kommunenavn = "Troms�", Fylkenummer = 55, Kommunenummer = 5401 },
                new { Id = 7, Fylkenavn = "Nordland", Kommunenavn = "Bod�", Fylkenummer = 18, Kommunenummer = 1804 },
                new { Id = 8, Fylkenavn = "Oslo", Kommunenavn = "Oslo", Fylkenummer = 3, Kommunenummer = 0301 },
                new { Id = 9, Fylkenavn = "Finnmark", Kommunenavn = "Vads�", Fylkenummer = 56, Kommunenummer = 5405 },
                new { Id = 10, Fylkenavn = "Vestfold og Telemark", Kommunenavn = "Skien", Fylkenummer = 40, Kommunenummer = 3807 }
            );

            // Insert data into Bruker
            modelBuilder.Entity<Bruker>().HasData(
                new { BrukerId = 1, Email = "bruker1@example.com", Passord = "passord123", BrukerType = "standard" },
                new { BrukerId = 2, Email = "bruker2@example.com", Passord = "passord123", BrukerType = "standard" },
                new { BrukerId = 3, Email = "bruker3@example.com", Passord = "passord123", BrukerType = "standard" },
                new { BrukerId = 4, Email = "bruker4@example.com", Passord = "passord123", BrukerType = "standard" },
                new { BrukerId = 5, Email = "bruker5@example.com", Passord = "passord123", BrukerType = "standard" },
                new { BrukerId = 6, Email = "bruker6@example.com", Passord = "passord123", BrukerType = "standard" },
                new { BrukerId = 7, Email = "bruker7@example.com", Passord = "passord123", BrukerType = "standard" },
                new { BrukerId = 8, Email = "bruker8@example.com", Passord = "passord123", BrukerType = "standard" },
                new { BrukerId = 9, Email = "bruker9@example.com", Passord = "passord123", BrukerType = "standard" },
                new { BrukerId = 10, Email = "bruker10@example.com", Passord = "passord123", BrukerType = "standard" },
                new { BrukerId = 11, Email = "ambulansen@test.com", Passord = "passord123", BrukerType = "spesial" },
                new { BrukerId = 12, Email = "saksbehandler@example.com", Passord = "passord123", BrukerType = "saksbehandler" },
                new { BrukerId = 13, Email = "admin@example.com", Passord = "passord123", BrukerType = "admin" },
                new { BrukerId = 14, Email = "OsloOslo@example.com", Passord = "passord123", BrukerType = "saksbehandler" },
                new { BrukerId = 15, Email = "VestlandBergen@example.com", Passord = "passord123", BrukerType = "saksbehandler" },
                new { BrukerId = 16, Email = "RogalandStavanger@example.com", Passord = "passord123", BrukerType = "saksbehandler" },
                new { BrukerId = 17, Email = "Tr�ndelagTrondheim@example.com", Passord = "passord123", BrukerType = "saksbehandler" },
                new { BrukerId = 18, Email = "VikenDrammen@example.com", Passord = "passord123", BrukerType = "saksbehandler" },
                new { BrukerId = 19, Email = "VikenFredrikstad@example.com", Passord = "passord123", BrukerType = "saksbehandler" },
                new { BrukerId = 20, Email = "AgderKristiansand@example.com", Passord = "passord123", BrukerType = "saksbehandler" },
                new { BrukerId = 21, Email = "RogalandSandnes@example.com", Passord = "passord123", BrukerType = "saksbehandler" },
                new { BrukerId = 22, Email = "TromsOgFinnmarkTroms�@example.com", Passord = "passord123", BrukerType = "saksbehandler" },
                new { BrukerId = 23, Email = "NordlandBod�@example.com", Passord = "passord123", BrukerType = "saksbehandler" }
            );

            // Insert data into Person
            modelBuilder.Entity<Person>().HasData(
                new { PersonId = 1, Fornavn = "Ola", Etternavn = "Nordmann", BrukerId = 1 },
                new { PersonId = 2, Fornavn = "Kari", Etternavn = "Nordmann", BrukerId = 2 },
                new { PersonId = 3, Fornavn = "Per", Etternavn = "Hansen", BrukerId = 3 },
                new { PersonId = 4, Fornavn = "Anne", Etternavn = "Larsen", BrukerId = 4 },
                new { PersonId = 5, Fornavn = "Nina", Etternavn = "Johansen", BrukerId = 5 },
                new { PersonId = 6, Fornavn = "Erik", Etternavn = "Berg", BrukerId = 6 },
                new { PersonId = 7, Fornavn = "Lise", Etternavn = "Olsen", BrukerId = 7 },
                new { PersonId = 8, Fornavn = "Hans", Etternavn = "Moen", BrukerId = 8 },
                new { PersonId = 9, Fornavn = "Mona", Etternavn = "Lie", BrukerId = 9 },
                new { PersonId = 10, Fornavn = "Tom", Etternavn = "Halvorsen", BrukerId = 10 },
                new { PersonId = 11, Fornavn = "Abu", Etternavn = "Lance", BrukerId = 11 },
                new { PersonId = 12, Fornavn = "Saks", Etternavn = "Behandler", BrukerId = 12 },
                new { PersonId = 13, Fornavn = "Adam", Etternavn = "Minh", BrukerId = 13 },
                new { PersonId = 14, Fornavn = "Ole", Etternavn = "Oslo", BrukerId = 14 },
                new { PersonId = 15, Fornavn = "Bj�rn", Etternavn = "Bergen", BrukerId = 15 },
                new { PersonId = 16, Fornavn = "Siri", Etternavn = "Stavanger", BrukerId = 16 },
                new { PersonId = 17, Fornavn = "Knut", Etternavn = "Trondheim", BrukerId = 17 },
                new { PersonId = 18, Fornavn = "Lena", Etternavn = "Drammen", BrukerId = 18 },
                new { PersonId = 19, Fornavn = "Marta", Etternavn = "Fredrikstad", BrukerId = 19 },
                new { PersonId = 20, Fornavn = "Nils", Etternavn = "Kristiansand", BrukerId = 20 },
                new { PersonId = 21, Fornavn = "Eva", Etternavn = "Sandnes", BrukerId = 21 },
                new { PersonId = 22, Fornavn = "Per", Etternavn = "Troms�", BrukerId = 22 },
                new { PersonId = 23, Fornavn = "Ingrid", Etternavn = "Bod�", BrukerId = 23 }
            );

            // Insert data into Ansatt
            modelBuilder.Entity<Ansatt>().HasData(
                new { AnsattId = 1, PersonId = 12, Kommunenummer = 0000, AnsettelsesDato = DateTime.Parse("2023-05-15") },
                new { AnsattId = 2, PersonId = 14, Kommunenummer = 301, AnsettelsesDato = DateTime.Parse("2024-01-15") },
                new { AnsattId = 3, PersonId = 15, Kommunenummer = 4601, AnsettelsesDato = DateTime.Parse("2024-02-20") },
                new { AnsattId = 4, PersonId = 16, Kommunenummer = 1101, AnsettelsesDato = DateTime.Parse("2024-03-10") },
                new { AnsattId = 5, PersonId = 17, Kommunenummer = 5001, AnsettelsesDato = DateTime.Parse("2024-04-05") },
                new { AnsattId = 6, PersonId = 18, Kommunenummer = 3005, AnsettelsesDato = DateTime.Parse("2024-05-12") },
                new { AnsattId = 7, PersonId = 19, Kommunenummer = 3004, AnsettelsesDato = DateTime.Parse("2024-06-18") },
                new { AnsattId = 8, PersonId = 20, Kommunenummer = 4204, AnsettelsesDato = DateTime.Parse("2024-07-25") },
                new { AnsattId = 9, PersonId = 21, Kommunenummer = 1108, AnsettelsesDato = DateTime.Parse("2024-08-30") },
                new { AnsattId = 10, PersonId = 22, Kommunenummer = 5401, AnsettelsesDato = DateTime.Parse("2024-09-14") },
                new { AnsattId = 11, PersonId = 23, Kommunenummer = 1804, AnsettelsesDato = DateTime.Parse("2024-10-22") }
            );

modelBuilder.Entity<Kart>().HasData(
    new { KartEndringId = 1, Koordsys = 4258, Tittel = "Veiskade", Beskrivelse = "Hull i veien på hovedgata. Dette hullet har blitt rapportert flere ganger og trenger umiddelbar oppmerksomhet fra vedlikeholdsteamet.", MapType = "Norge kart", RapportType = "Veiskade", SteddataId = 1, FilePath = (string?)null },
    new { KartEndringId = 2, Koordsys = 4258, Tittel = "Oversvømt område", Beskrivelse = "Området ved elvebredden har vært oversvømt i flere dager, og vannstanden ser ut til å fortsette å stige.", MapType = "Norge kart", RapportType = "Oversvømmelse", SteddataId = 2, FilePath = (string?)null },
    new { KartEndringId = 3, Koordsys = 4258, Tittel = "Fjellskred", Beskrivelse = "Steinras som blokkerer en viktig sti i fjellområdet, og dette skaper en stor risiko for turgåere og fjellklatrere.", MapType = "Turkart", RapportType = "Skredfare", SteddataId = 3, FilePath = (string?)null },
    new { KartEndringId = 4, Koordsys = 4258, Tittel = "Skadet sti", Beskrivelse = "Stien er kraftig overgrodd med planter, og det er tydelige tegn på erosjon langs hele strekningen.", MapType = "Turkart", RapportType = "Stivedlikehold", SteddataId = 4, FilePath = (string?)null },
    new { KartEndringId = 5, Koordsys = 4258, Tittel = "Båtvrak", Beskrivelse = "Flere gamle båtvrak har samlet seg langs kystlinjen. Dette kan være farlig for mindre båter og svømmere.", MapType = "Sjøkart", RapportType = "Ryddeaksjon", SteddataId = 5, FilePath = (string?)null },
    new { KartEndringId = 6, Koordsys = 4258, Tittel = "Trafikkulykke", Beskrivelse = "Alvorlig trafikkulykke på motorveien med flere kjøretøy involvert. Krever umiddelbar rydding for å unngå kø.", MapType = "Norge kart", RapportType = "Trafikkulykke", SteddataId = 6, FilePath = (string?)null },
    new { KartEndringId = 7, Koordsys = 4258, Tittel = "Snøras", Beskrivelse = "Snøras i fjellområdet som har blokkert veien og kan utgjøre en fare for kommende trafikk.", MapType = "Turkart", RapportType = "Snørasfare", SteddataId = 7, FilePath = (string?)null },
    new { KartEndringId = 8, Koordsys = 4258, Tittel = "Sykkelsti skadet", Beskrivelse = "Sykkelstien har store sprekker og hull som gjør det vanskelig for syklister å bruke den trygt.", MapType = "Norge kart", RapportType = "Sykkelsti reparasjon", SteddataId = 8, FilePath = (string?)null },
    new { KartEndringId = 9, Koordsys = 4258, Tittel = "Båthavn", Beskrivelse = "Båthavnen er overfylt med båter, noe som gjør det vanskelig for nye båter å legge til kai eller parkere.", MapType = "Sjøkart", RapportType = "Overfylte båtplasser", SteddataId = 9, FilePath = (string?)null },
    new { KartEndringId = 10, Koordsys = 4258, Tittel = "Fiskefelt", Beskrivelse = "Fiskefeltet er overbeskattet, og det er behov for strengere regulering for å bevare fiskebestanden.", MapType = "Sjøkart", RapportType = "Fiskeriforvaltning", SteddataId = 10, FilePath = (string?)null }
);

            // Insert data into Rapport
            modelBuilder.Entity<Rapport>().HasData(
                new { RapportId = 1, RapportStatus = "U�pnet", Opprettet = DateTime.Now, TildelAnsattId = 1, PersonId = 1, KartEndringId = 1 },
                new { RapportId = 2, RapportStatus = "Under behandling", Opprettet = DateTime.Now, TildelAnsattId = 1, PersonId = 2, KartEndringId = 2 },
                new { RapportId = 3, RapportStatus = "Avklart", Opprettet = DateTime.Now, TildelAnsattId = 1, PersonId = 3, KartEndringId = 3 },
                new { RapportId = 4, RapportStatus = "U�pnet", Opprettet = DateTime.Now, TildelAnsattId = 1, PersonId = 4, KartEndringId = 4 },
                new { RapportId = 5, RapportStatus = "Under behandling", Opprettet = DateTime.Now, TildelAnsattId = 1, PersonId = 5, KartEndringId = 5 },
                new { RapportId = 6, RapportStatus = "Avklart", Opprettet = DateTime.Now, TildelAnsattId = 1, PersonId = 6, KartEndringId = 6 },
                new { RapportId = 7, RapportStatus = "U�pnet", Opprettet = DateTime.Now, TildelAnsattId = 1, PersonId = 7, KartEndringId = 7 },
                new { RapportId = 8, RapportStatus = "Under behandling", Opprettet = DateTime.Now, TildelAnsattId = 1, PersonId = 8, KartEndringId = 8 },
                new { RapportId = 9, RapportStatus = "Avklart", Opprettet = DateTime.Now, TildelAnsattId = 1, PersonId = 9, KartEndringId = 9 },
                new { RapportId = 10, RapportStatus = "U�pnet", Opprettet = DateTime.Now, TildelAnsattId = 1, PersonId = 10, KartEndringId = 10 }
            );

            // Insert data into Koordinater
            modelBuilder.Entity<Koordinater>().HasData(
                new { KoordinatId = 1, KartEndringId = 1, Rekkefolge = 1, Nord = 59.9139, Ost = 10.7522 },
                new { KoordinatId = 2, KartEndringId = 1, Rekkefolge = 2, Nord = 59.9140, Ost = 10.7523 },

                // Coordinates for KartEndringId 2 (Vestland - Bergen)
                new { KoordinatId = 3, KartEndringId = 2, Rekkefolge = 1, Nord = 60.3913, Ost = 5.3221 },
                new { KoordinatId = 4, KartEndringId = 2, Rekkefolge = 2, Nord = 60.3915, Ost = 5.3224 },
                new { KoordinatId = 5, KartEndringId = 2, Rekkefolge = 3, Nord = 60.3914, Ost = 5.3223 },

                // Coordinates for KartEndringId 3 (Tr�ndelag - Trondheim)
                new { KoordinatId = 6, KartEndringId = 3, Rekkefolge = 1, Nord = 63.4305, Ost = 10.3951 },
                new { KoordinatId = 7, KartEndringId = 3, Rekkefolge = 2, Nord = 63.4306, Ost = 10.3952 },
                new { KoordinatId = 8, KartEndringId = 3, Rekkefolge = 3, Nord = 63.4307, Ost = 10.3953 },
                new { KoordinatId = 9, KartEndringId = 3, Rekkefolge = 4, Nord = 63.4308, Ost = 10.3954 },

                // Coordinates for KartEndringId 4 (Agder - Kristiansand)
                new { KoordinatId = 10, KartEndringId = 4, Rekkefolge = 1, Nord = 58.1467, Ost = 7.9946 },
                new { KoordinatId = 11, KartEndringId = 4, Rekkefolge = 2, Nord = 58.1468, Ost = 7.9947 },

                // Coordinates for KartEndringId 5 (Innlandet - V�g�mo)
                new { KoordinatId = 12, KartEndringId = 5, Rekkefolge = 1, Nord = 61.8735, Ost = 9.0946 },
                new { KoordinatId = 13, KartEndringId = 5, Rekkefolge = 2, Nord = 61.8736, Ost = 9.0947 },
                new { KoordinatId = 14, KartEndringId = 5, Rekkefolge = 3, Nord = 61.8737, Ost = 9.0948 },
                new { KoordinatId = 15, KartEndringId = 5, Rekkefolge = 4, Nord = 61.8738, Ost = 9.0949 },
                new { KoordinatId = 16, KartEndringId = 5, Rekkefolge = 5, Nord = 61.8739, Ost = 9.0950 },
                new { KoordinatId = 17, KartEndringId = 5, Rekkefolge = 6, Nord = 61.8740, Ost = 9.0951 },

                // Coordinates for KartEndringId 6 (Troms og Finnmark - Troms�)
                new { KoordinatId = 18, KartEndringId = 6, Rekkefolge = 1, Nord = 69.6492, Ost = 18.9553 },
                new { KoordinatId = 19, KartEndringId = 6, Rekkefolge = 2, Nord = 69.6493, Ost = 18.9554 },
                new { KoordinatId = 20, KartEndringId = 6, Rekkefolge = 3, Nord = 69.6494, Ost = 18.9555 },

                // Coordinates for KartEndringId 7 (Nordland - Lofoten)
                new { KoordinatId = 21, KartEndringId = 7, Rekkefolge = 1, Nord = 68.4392, Ost = 17.4276 },
                new { KoordinatId = 22, KartEndringId = 7, Rekkefolge = 2, Nord = 68.4393, Ost = 17.4277 },

                // Coordinates for KartEndringId 8 (Oslo - East)
                new { KoordinatId = 23, KartEndringId = 8, Rekkefolge = 1, Nord = 59.9141, Ost = 10.7524 },
                new { KoordinatId = 24, KartEndringId = 8, Rekkefolge = 2, Nord = 59.9142, Ost = 10.7525 },

                // Coordinates for KartEndringId 9 (Finnmark - Kirkenes)
                new { KoordinatId = 25, KartEndringId = 9, Rekkefolge = 1, Nord = 70.0738, Ost = 29.7492 },
                new { KoordinatId = 26, KartEndringId = 9, Rekkefolge = 2, Nord = 70.0739, Ost = 29.7493 },
                new { KoordinatId = 27, KartEndringId = 9, Rekkefolge = 3, Nord = 70.0740, Ost = 29.7494 },
                new { KoordinatId = 28, KartEndringId = 9, Rekkefolge = 4, Nord = 70.0741, Ost = 29.7495 },

                // Coordinates for KartEndringId 10 (Vestfold og Telemark - Skien)
                new { KoordinatId = 29, KartEndringId = 10, Rekkefolge = 1, Nord = 59.0578, Ost = 10.0364 },
                new { KoordinatId = 30, KartEndringId = 10, Rekkefolge = 2, Nord = 59.0579, Ost = 10.0365 }
            );

            // Insert data into Meldinger
            modelBuilder.Entity<Meldinger>().HasData(
                // Meldinger for RapportId 1
                new { MeldingsId = 1, RapportId = 1, SenderPersonId = 12, MottakerPersonId = 1, Innhold = "Hei Ola, vi trenger mer informasjon om veiskaden du rapporterte.", Tidsstempel = DateTime.Now, Status = "sendt" },
                new { MeldingsId = 2, RapportId = 1, SenderPersonId = 1, MottakerPersonId = 12, Innhold = "Hei, her er de detaljerte opplysningene om veiskaden.", Tidsstempel = DateTime.Now, Status = "sendt" },

                // Meldinger for RapportId 2
                new { MeldingsId = 3, RapportId = 2, SenderPersonId = 12, MottakerPersonId = 2, Innhold = "Kari, kan du bekrefte oversv�mmelsen i omr�det du rapporterte?", Tidsstempel = DateTime.Now, Status = "sendt" },
                new { MeldingsId = 4, RapportId = 2, SenderPersonId = 2, MottakerPersonId = 12, Innhold = "Bekreftet, omr�det er fortsatt oversv�mt.", Tidsstempel = DateTime.Now, Status = "sendt" },

                // Meldinger for RapportId 3
                new { MeldingsId = 5, RapportId = 3, SenderPersonId = 12, MottakerPersonId = 3, Innhold = "Per, vi trenger oppdateringer om fjellskredet.", Tidsstempel = DateTime.Now, Status = "sendt" },
                new { MeldingsId = 6, RapportId = 3, SenderPersonId = 3, MottakerPersonId = 12, Innhold = "Fjellskredet er n� under overv�king.", Tidsstempel = DateTime.Now, Status = "sendt" },

                // Meldinger for RapportId 4
                new { MeldingsId = 7, RapportId = 4, SenderPersonId = 12, MottakerPersonId = 4, Innhold = "Anne, kan du sende bilder av den skadede stien?", Tidsstempel = DateTime.Now, Status = "sendt" },
                new { MeldingsId = 8, RapportId = 4, SenderPersonId = 4, MottakerPersonId = 12, Innhold = "Selvf�lgelig, her er bildene.", Tidsstempel = DateTime.Now, Status = "sendt" },

                // Meldinger for RapportId 5
                new { MeldingsId = 9, RapportId = 5, SenderPersonId = 12, MottakerPersonId = 5, Innhold = "Nina, har du innsikt i b�tvrakene?", Tidsstempel = DateTime.Now, Status = "sendt" },
                new { MeldingsId = 10, RapportId = 5, SenderPersonId = 5, MottakerPersonId = 12, Innhold = "Ja, vi trenger assistanse for rydding.", Tidsstempel = DateTime.Now, Status = "sendt" },

                // Meldinger for RapportId 6
                new { MeldingsId = 11, RapportId = 6, SenderPersonId = 12, MottakerPersonId = 6, Innhold = "Erik, vi har behov for rapporter om trafikkulykken.", Tidsstempel = DateTime.Now, Status = "sendt" },
                new { MeldingsId = 12, RapportId = 6, SenderPersonId = 6, MottakerPersonId = 12, Innhold = "Rapportene er under arbeid og vil bli levert snart.", Tidsstempel = DateTime.Now, Status = "sendt" },

                // Meldinger for RapportId 7
                new { MeldingsId = 13, RapportId = 7, SenderPersonId = 12, MottakerPersonId = 7, Innhold = "Lise, sn�rasen er kritisk, kan du igangsette tiltak?", Tidsstempel = DateTime.Now, Status = "sendt" },
                new { MeldingsId = 14, RapportId = 7, SenderPersonId = 7, MottakerPersonId = 12, Innhold = "Tiltak er iverksatt for � h�ndtere sn�rasen.", Tidsstempel = DateTime.Now, Status = "sendt" },

                // Meldinger for RapportId 8
                new { MeldingsId = 15, RapportId = 8, SenderPersonId = 12, MottakerPersonId = 8, Innhold = "Hans, vi trenger mer informasjon om sykkelsti-skaden.", Tidsstempel = DateTime.Now, Status = "sendt" },
                new { MeldingsId = 16, RapportId = 8, SenderPersonId = 8, MottakerPersonId = 12, Innhold = "Her er de n�dvendige detaljene.", Tidsstempel = DateTime.Now, Status = "sendt" },

                // Meldinger for RapportId 9
                new { MeldingsId = 17, RapportId = 9, SenderPersonId = 12, MottakerPersonId = 9, Innhold = "Mona, kan du overv�ke fiskefeltet?", Tidsstempel = DateTime.Now, Status = "sendt" },
                new { MeldingsId = 18, RapportId = 9, SenderPersonId = 9, MottakerPersonId = 12, Innhold = "Fiskefeltet overv�kes kontinuerlig.", Tidsstempel = DateTime.Now, Status = "sendt" },

                // Meldinger for RapportId 10
                new { MeldingsId = 19, RapportId = 10, SenderPersonId = 12, MottakerPersonId = 10, Innhold = "Tom, vi trenger data om fiskeriforvaltningen.", Tidsstempel = DateTime.Now, Status = "sendt" },
                new { MeldingsId = 20, RapportId = 10, SenderPersonId = 10, MottakerPersonId = 12, Innhold = "Dataene er samlet og kan sendes.", Tidsstempel = DateTime.Now, Status = "sendt" }
            );
        }
    }
}
