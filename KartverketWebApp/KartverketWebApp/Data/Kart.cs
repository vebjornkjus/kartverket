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
        public string MapType { get; set; }
        public string RapportType { get; set; }

        // Navigation Properties
        public ICollection<Koordinater> Koordinater { get; set; } // One-to-Many with Koordinater
        public ICollection<Rapport> Rapporter { get; set; } // One-to-Many with Rapport
    }

}

