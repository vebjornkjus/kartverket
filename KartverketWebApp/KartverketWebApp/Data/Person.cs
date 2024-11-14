using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }

        public string Fornavn { get; set; }
        public string Etternavn { get; set; }

        public int? BrukerId { get; set; }

        // Navigation Properties
        public Bruker Bruker { get; set; }
        public ICollection<Rapport> Rapporter { get; set; } // One-to-Many med Rapport
        public ICollection<Ansatt> Ansatt { get; set; } // One-to-Many med Rapport
    }
}
