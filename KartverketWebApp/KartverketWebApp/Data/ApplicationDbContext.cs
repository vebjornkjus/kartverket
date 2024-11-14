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
            // Sikre at Identity-konfigurasjonen blir kjørt først
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
                .WithMany(p => p.Ansatt) // Legg til ICollection<Ansatt> i Person-klassen hvis nødvendig
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
        }
    }
}
