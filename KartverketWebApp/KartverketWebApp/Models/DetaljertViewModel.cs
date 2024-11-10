using KartverketWebApp.Data;

namespace KartverketWebApp.Models
{
    public class DetaljertViewModel
    {
        public Rapport Rapport { get; set; }
        public Kart Kart { get; set; }
        public Person Person { get; set; }
        public StednavnViewModel Stednavn { get; set; }

    }


}
