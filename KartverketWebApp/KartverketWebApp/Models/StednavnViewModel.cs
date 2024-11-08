namespace KartverketWebApp.Models
{
    public class StednavnViewModel
	{
        public int KartEndringId { get; set; }  // Link to Kart
        public string? Fylkesnavn { get; set; }

		public string? Fylkesnummer { get; set; }

		public string? Kommunenavn { get; set; }

		public string? Kommunenummer { get; set; }
	}
}
