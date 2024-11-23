using Microsoft.AspNetCore.Mvc;
using KartverketWebApp.Models; // Husk å bruke riktig namespace for din AdminViewModel

namespace KartverketWebApp.Controllers
{
    public class AdminController : Controller
    {
        // Denne handler for å vise oversikten
        public IActionResult BrukerOversikt()
        {
            // Lag eller hent nødvendige data for BrukerOversikt
            var viewModel = new AdminViewModel
            {
                // Her setter du opp data som trengs i BrukerOversikt
                // Eksempel: viewModel.Brukere = dbContext.Brukere.ToList();
            };
            return View("~/Views/Home/Admin/BrukerOversikt.cshtml", viewModel); // Sender viewModel til visningen
        }
    }
}