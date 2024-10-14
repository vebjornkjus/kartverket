namespace KartverketWebApp.Models
{
    public class CombinedViewModel
    {
        public List<PositionModel> Positions { get; set; }
        public List<StednavnViewModel> Stednavn { get; set; }

        public CombinedViewModel()
        {
            Positions = new List<PositionModel>();
            Stednavn = new List<StednavnViewModel>();
        }
    }
}
