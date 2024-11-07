using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Rapport
    {
        [Key]
        public int RapportId { get; set; }

        public string RapportStatus { get; set; }
        public DateTime Opprettet { get; set; }

        public int PersonId { get; set; } // Foreign key to Person
        public int KartEndringId { get; set; } // Foreign key to Kart

        // Navigation Properties
        public Person Person { get; set; }
        public Kart Kart { get; set; }
    }
}
