using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Steddata
    {
        [Key]
        public int Id { get; set; }

        public string Fylkenavn { get; set; }
        public string Kommunenavn { get; set; }
        public int Fylkenummer { get; set; }
        public int Kommunenummer { get; set; }

        // Navigation Properties
        public Kart Kart { get; set; } // En-til-En med Kart
    }
}
