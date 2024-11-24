using KartverketWebApp.Controllers;
using KartverketWebApp.Data;

namespace KartverketWebApp.Models
{
    public class TidligereRapporterModel
    {
            public Person Person { get; set; }
            public Bruker Bruker { get; set; }
            public List<RapportViewModel> AvklartRapporter { get; set; } = new();
            public List<RapportViewModel> FjernetRapporter { get; set; } = new();
    }
}
