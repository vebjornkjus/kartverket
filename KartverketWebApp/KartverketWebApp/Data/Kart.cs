using System.ComponentModel.DataAnnotations;
using static KartverketWebApp.Models.PositionModel;

namespace KartverketWebApp.Data
{
    public class Kart
    {
        [Key]
        public int KartEndringId { get; set; }

        public string Tittel { get; set; }
        public string Beskrivelse { get; set; }
        public int Koordsys { get; set; }
        public int SteddataId { get; set; } // Foreign key til Steddata
        public string MapType { get; set; }
        public string RapportType { get; set; }

        // Navigation Properties
        public Steddata Steddata { get; set; }
        public ICollection<Koordinater> Koordinater { get; set; } // One-to-Many med Koordinater
        public ICollection<Rapport> Rapporter { get; set; } // One-to-Many med Rapport
    }

}

