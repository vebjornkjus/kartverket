using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Rapport
    {
        [Key]
        public int RapportId { get; set; }

        public string RapportStatus { get; set; }
        public DateTime Opprettet { get; set; }

        public int? TildelAnsattId { get; set; } 
        public int PersonId { get; set; }
        public int KartEndringId { get; set; } 

        public DateTime? BehandletDato { get; set; } 

        // Navigation Properties
        public Ansatt TildelAnsatt { get; set; }
        public Person Person { get; set; }
        public Kart Kart { get; set; }
        public ICollection<Meldinger> Meldinger { get; set; } // One-to-Many med Meldinger
    }
}
