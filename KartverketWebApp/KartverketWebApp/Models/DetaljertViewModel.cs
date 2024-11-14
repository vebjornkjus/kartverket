using KartverketWebApp.Data;

namespace KartverketWebApp.Models
{
    public class DetaljertViewModel
    {
        public Rapport Rapport { get; set; }
        public Kart Kart { get; set; }
        public Person Person { get; set; }
        public Bruker Bruker { get; set; } 
        public Steddata Steddata { get; set; }
    }

}