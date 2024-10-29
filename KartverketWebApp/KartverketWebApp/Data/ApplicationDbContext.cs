namespace KartverketWebApp.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Kart> Kart { get; set; }
        public DbSet<Koordinater> Koordinater { get; set; }
        public DbSet<Rapport> Rapport { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Bruker> Bruker { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kart>()
                .HasOne(k => k.Koordinater)
                .WithMany()
                .HasForeignKey(k => k.KoordinaterId);

            modelBuilder.Entity<Rapport>()
                .HasOne(r => r.Person)
                .WithMany(p => p.Rapporter)
                .HasForeignKey(r => r.PersonId);

            modelBuilder.Entity<Rapport>()
                .HasOne(r => r.Kart)
                .WithMany(k => k.Rapporter)
                .HasForeignKey(r => r.KartEndringId);

            modelBuilder.Entity<Person>()
                .HasOne(p => p.Bruker)
                .WithMany(b => b.Personer)
                .HasForeignKey(p => p.BrukerId);
        }
    }

}
