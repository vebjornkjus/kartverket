using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Koordinater
    {
        [Key]
        public int KoordinatId { get; set; }

        public int Rekkefolge { get; set; }
        public double Nord { get; set; }
        public double Ost { get; set; }

        public int KartEndringId { get; set; } // Foreign key to Kart

        // Navigation Property
        public Kart Kart { get; set; }
    }

}


