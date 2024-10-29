using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Bruker
    {
        [Key]
        public int BrukerId { get; set; }

        public string Brukernavn { get; set; }
        public string Passord { get; set; }
        public string BrukerType { get; set; }

        public ICollection<Person> Personer { get; set; }
    }
}
