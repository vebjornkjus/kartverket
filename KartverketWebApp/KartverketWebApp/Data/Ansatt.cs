using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Ansatt
    {
        [Key]
        public int AnsattId { get; set; }

        public int PersonId { get; set; } // Foreign key til Person
        public int Kommunenummer { get; set; }
        public DateTime? AnsettelsesDato { get; set; }

        // Navigation Properties
        public Person Person { get; set; }
        public ICollection<Rapport> Rapporter { get; set; } // One-to-Many med Rapport
    }
}
