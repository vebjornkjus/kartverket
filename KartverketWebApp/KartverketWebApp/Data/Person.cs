using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }

        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Email { get; set; }
        public int Telefon { get; set; }
        public int? BrukerId { get; set; }

        public Bruker Bruker { get; set; }
        public ICollection<Rapport> Rapporter { get; set; }
    }
}
