using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Rapport
    {
        [Key]
        public int RapportId { get; set; }

        public int PersonId { get; set; }
        public int KartEndringId { get; set; }
        public string RapportStatus { get; set; }
        public DateTime Opprettet { get; set; }

        public Person Person { get; set; }
        public Kart Kart { get; set; }
    }
}
