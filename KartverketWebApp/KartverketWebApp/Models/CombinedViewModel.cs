using KartverketWebApp.Controllers;
using KartverketWebApp.Data;
using KartverketWebApp.Models;

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
        public List<Meldinger> Meldinger { get; set; }
        public List<SammtaleModel> SammtaleModel { get; set; }
        public List<TildelRapportModel> TildelRapportModel { get; set; }
        public List<TidligereRapporterModel> TidligereRapporterModel { get; set; }




        public List<RapportViewModel> AvklartRapporter { get; set; }
        public List<RapportViewModel> FjernetRapporter { get; set; }

        public CombinedViewModel()
        {
            Positions = new List<PositionModel>();
            Stednavn = new List<StednavnViewModel>();
            Rapporter = new List<Rapport>();
            AvklartRapporter = new List<RapportViewModel>();
            FjernetRapporter = new List<RapportViewModel>();
        }

    }
    }

