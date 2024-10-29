using System.ComponentModel.DataAnnotations;
using static KartverketWebApp.Models.PositionModel;

namespace KartverketWebApp.Data
{
    public class Kart
    {
        [Key]
        public int KartEndringId { get; set; }

        public int? KoordinaterId { get; set; }
        public int Koordsys { get; set; }
        public string Tittel { get; set; }
        public string Beskrivelse { get; set; }
        public string MapType { get; set; }
        public string RapportType { get; set; }

        public Koordinater Koordinater { get; set; }
        public ICollection<Rapport> Rapporter { get; set; }
    }
}
