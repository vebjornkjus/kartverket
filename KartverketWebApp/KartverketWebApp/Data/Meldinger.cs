using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Meldinger
    {
        [Key]
        public int MeldingsId { get; set; }

        public int RapportId { get; set; }
        public int SenderPersonId { get; set; }
        public int MottakerPersonId { get; set; }
        public string Innhold { get; set; }
        public DateTime Tidsstempel { get; set; }
        public string Status { get; set; }

        // Navigation Properties
        public Rapport Rapport { get; set; }
        public Person Sender { get; set; }
        public Person Mottaker { get; set; }
    }
}
