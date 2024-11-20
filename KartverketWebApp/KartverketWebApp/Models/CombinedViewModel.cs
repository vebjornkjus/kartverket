using KartverketWebApp.Data;

namespace KartverketWebApp.Models
{
    public class CombinedViewModel
    {
        public List<PositionModel> Positions { get; set; }
        public List<StednavnViewModel> Stednavn { get; set; }
        public List<Rapport> Rapporter { get; set; }
        public List<Kart> KartData { get; set; }
        public List<Koordinater> KoordinatData { get; set; }

        public List<Rapport> ActiveRapporter { get; set; }
        public List<Rapport> ResolvedRapporter { get; set; }

        public CombinedViewModel()
        {
            Positions = new List<PositionModel>();
            Stednavn = new List<StednavnViewModel>();
            Rapporter = new List<Rapport>();

        }
    }
}
