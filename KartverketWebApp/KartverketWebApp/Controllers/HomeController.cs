using KartverketWebApp.Models;
using KartverketWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace KartverketWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStednavn _stednavnService;
        private static List<PositionModel> positions = new List<PositionModel>();

        public HomeController(ILogger<HomeController> logger, IStednavn stedsnavnService)
        {
            _logger = logger;
            _stednavnService = stedsnavnService;
        }

        public IActionResult TakkRapport()
        {
            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Return the Index view, no model passed
            return View();
        }

        [HttpPost]
        public IActionResult Index(PositionModel model)
        {       
            if (ModelState.IsValid)
            {
                // Add the valid model to the positions list
                positions.Add(model);

                // Redirect to the CorrectionsOverview action to display the updated list
                return RedirectToAction("CorrectionsOverview");
            }
            ViewBag.ErrorMessage = "Failed to retrieve location data.";
            return View(model); // Return the same view with model errors
        }

        public IActionResult CorrectionsOverview()
        {
            // Prepare the CombinedViewModel to be passed to the view
            var viewModel = new CombinedViewModel
            {
                Positions = positions ?? new List<PositionModel>(), // Ensure positions is not null
                Stednavn = new StednavnViewModel() // Initialize as needed
            };

            _logger.LogInformation($"Stednavn data: Fylkesnavn = {viewModel.Stednavn?.Fylkesnavn}, Kommunenavn = {viewModel.Stednavn?.Kommunenavn}");

            return View(viewModel); // Return the view with CombinedViewModel
        }

        [HttpPost]
        public async Task<IActionResult> Stedsnavn(double nord, double ost, int koordsys)
        {
            // Fetch data from the API service
            var stednavnResponse = await _stednavnService.GetStednavnAsync(nord, ost, koordsys);


            // Check if data exists
            if (stednavnResponse != null)
            {
                // Map the response to StednavnViewModel
                var viewModel = new StednavnViewModel
                {
                    Fylkesnavn = stednavnResponse.Fylkesnavn,
                    Fylkesnummer = stednavnResponse.Fylkesnummer,
                    Kommunenavn = stednavnResponse.Kommunenavn,
                    Kommunenummer = stednavnResponse.Kommunenummer
                };

                return View("CorrectionsOverview", viewModel); // Ensure this view is returned
            }
            else
            {
                ViewData["Error"] = $"No results found for coordinates: nord {nord}, ost {ost}, Koordsys {koordsys}.";
                return View("Index");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
