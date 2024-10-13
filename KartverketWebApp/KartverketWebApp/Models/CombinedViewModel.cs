namespace KartverketWebApp.Models
{
    public class CombinedViewModel
    {
        public List<PositionModel> Positions { get; set; }
        public StednavnViewModel Stednavn { get; set; }

        public CombinedViewModel()
        {
            Positions = new List<PositionModel>();
            Stednavn = new StednavnViewModel();
        }
    }
}
