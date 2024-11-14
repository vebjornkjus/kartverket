using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Rapport
    {
        [Key]
        public int RapportId { get; set; }

        public string RapportStatus { get; set; }
        public DateTime Opprettet { get; set; }

        public int TildelAnsattId { get; set; } // Foreign key til Ansatt
        public int PersonId { get; set; } // Foreign key til Person
        public int KartEndringId { get; set; } // Foreign key til Kart

        // Navigation Properties
        public Ansatt TildelAnsatt { get; set; }
        public Person Person { get; set; }
        public Kart Kart { get; set; }
        public ICollection<Meldinger> Meldinger { get; set; } // One-to-Many med Meldinger
    }
}
