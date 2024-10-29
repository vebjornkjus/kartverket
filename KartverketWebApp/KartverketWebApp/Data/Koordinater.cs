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

        public ICollection<Kart> Kart { get; set; }
    }

}

