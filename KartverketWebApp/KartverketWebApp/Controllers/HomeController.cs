using KartverketWebApp.Models;
using KartverketWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace KartverketWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStednavn _stednavnService;
        private static List<PositionModel> positions = new List<PositionModel>();
        private static List<StednavnViewModel> stednavn = new List<StednavnViewModel>();

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
        public async Task<IActionResult> Index(PositionModel positionModel, StednavnViewModel stednavnModel)
        {
            if (ModelState.IsValid)
            {
                // Add the valid position model to the positions list
                positions.Add(positionModel);

                // Call the Stednavn API to get the geographical data
                var stednavnResponse = await _stednavnService.GetStednavnAsync(positionModel.Nord, positionModel.Ost, positionModel.Koordsys);

                if (stednavnResponse != null)
                {
                    _logger.LogInformation("Received StednavnResponse from API.");
                    _logger.LogInformation($"Fylkesnavn: {stednavnResponse.Fylkesnavn}");
                    _logger.LogInformation($"Fylkesnummer: {stednavnResponse.Fylkesnummer}");
                    _logger.LogInformation($"Kommunenavn: {stednavnResponse.Kommunenavn}");
                    _logger.LogInformation($"Kommunenummer: {stednavnResponse.Kommunenummer}");

                    stednavn.Add(new StednavnViewModel
                    {
                        Fylkesnavn = stednavnResponse.Fylkesnavn,
                        Fylkesnummer = stednavnResponse.Fylkesnummer,
                        Kommunenavn = stednavnResponse.Kommunenavn,
                        Kommunenummer = stednavnResponse.Kommunenummer
                    });

                    // Redirect to the CorrectionsOverview action to display the updated list
                    return RedirectToAction("CorrectionsOverview");
                }
                else
                {
                    ViewData["Error"] = $"No results found for coordinates: nord {positionModel.Nord}, ost {positionModel.Ost}, Koordsys {positionModel.Koordsys}.";
                    return View("Index");
                }
            }

            ViewBag.ErrorMessage = "Failed to retrieve location data.";
            return View(positionModel);
        }


        public IActionResult CorrectionsOverview()
        {
            // Prepare the CombinedViewModel to be passed to the view
            var viewModel = new CombinedViewModel
            {
                Positions = positions ?? new List<PositionModel>(), // Ensure positions is not null
                Stednavn = stednavn ?? new List<StednavnViewModel>() // Initialize as needed
            };

            foreach (var navn in viewModel.Stednavn)
            {
                _logger.LogInformation($"Stednavn data: Fylkesnavn = {navn.Fylkesnavn}, Kommunenavn = {navn.Kommunenavn}");
            }

            return View(viewModel); // Return the view with CombinedViewModel
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
