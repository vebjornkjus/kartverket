using KartverketWebApp.Data;

namespace KartverketWebApp.Models
{
    public class MinSideViewModel
    {
        public Bruker Bruker { get; set; }
        public Person Person { get; set; }
        public List<Rapport> Rapporter { get; set; }
    }
}
