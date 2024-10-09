namespace KartverketWebApp.Models
{
    public class PositionModel

    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int Koordsys { get; set; }

        public string? Brukernavn { get; set; }

        public string? Description { get; set; }

        public string? Map_type { get; set; }

        public string? Rapport_type { get; set; }

        public string? Fylkesnavn { get; set; }
        public string? Fylkesnummer { get; set; }
        public string? Kommunenavn { get; set; }
        public string? Kommunenummer { get; set; }
    }
}

