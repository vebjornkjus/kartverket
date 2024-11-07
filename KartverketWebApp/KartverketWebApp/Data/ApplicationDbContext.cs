using Microsoft.EntityFrameworkCore;
namespace KartverketWebApp.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Kart> Kart { get; set; }
        public DbSet<Koordinater> Koordinater { get; set; }
        public DbSet<Rapport> Rapport { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Bruker> Bruker { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-many relationship between Kart and Koordinater
            base.OnModelCreating(modelBuilder);  // Sikre Identity-konfigurasjon

            modelBuilder.Entity<Kart>()
                .HasMany(k => k.Koordinater)
                .WithOne(ko => ko.Kart)
                .HasForeignKey(ko => ko.KartEndringId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many relationship between Kart and Rapport
            modelBuilder.Entity<Kart>()
                .HasMany(k => k.Rapporter)
                .WithOne(r => r.Kart)
                .HasForeignKey(r => r.KartEndringId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many relationship between Person and Rapport
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Rapporter)
                .WithOne(r => r.Person)
                .HasForeignKey(r => r.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure optional one-to-many relationship between Bruker and Person
            modelBuilder.Entity<Bruker>()
                .HasMany(b => b.Personer)
                .WithOne(p => p.Bruker)
                .HasForeignKey(p => p.BrukerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Other configurations can go here...
        }
    }
}
