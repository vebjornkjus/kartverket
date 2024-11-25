namespace KartverketWebApp.Models
{
    public class AdminViewModel
    {
        public int BrukerId { get; set; }
        public string Email { get; set; }
        public string BrukerType { get; set; }
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public int? Kommunenummer { get; set; }
    }

}
