using System.ComponentModel.DataAnnotations;

namespace KartverketWebApp.Data
{
    public class Bruker
    {
        internal string IdentityUserId;

        [Key]
        public int BrukerId { get; set; }

        public string Email { get; set; } // Endret fra Brukernavn til Email
        public string Passord { get; set; }
        public string BrukerType { get; set; }

        // Navigation Property
        public ICollection<Person> Personer { get; set; } // One-to-Many med Person
    }
}