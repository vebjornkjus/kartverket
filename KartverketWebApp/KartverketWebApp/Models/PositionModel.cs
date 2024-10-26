
namespace KartverketWebApp.Models
{
    public class PositionModel

    {
        public string? Kart_endring_id { get; set; }
        public List<Coordinate> Coordinates { get; set; } = new List<Coordinate>();

        public int Koordsys { get; set; }

        public string? Tittel { get; set; }

        public string? Description { get; set; }

        public string? Map_type { get; set; }

        public string? Rapport_type { get; set; }

        public class Coordinate
        {
            public double Nord { get; set; }
            public double Ost { get; set; }
        }

    }


}

