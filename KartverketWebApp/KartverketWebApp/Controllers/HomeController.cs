using KartverketWebApp.Models;
using KartverketWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json; // Make sure to include this for JSON serialization
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace KartverketWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly StednavnService _stednavnService;

        private readonly ILogger<HomeController> _logger;

        private static List<PositionModel> positions = new List<PositionModel>();

        // Single constructor to inject both StednavnService and ILogger
        public HomeController(ILogger<HomeController> logger, StednavnService stednavnService)
        {
            _logger = logger;
            _stednavnService = stednavnService;
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetLocationData(double latitude, double longitude, int koordsys)
        {
            // Use the injected LocationService to call the API
            var steddata = await _stednavnService.GetLocationData(latitude, longitude, koordsys);

            if (steddata != null)
            {
                // Pass the location data to the view
                return View("CorrectionsOverview", steddata);
            }

            // Handle the error case
            ViewBag.ErrorMessage = "Failed to retrieve location data.";
            return View("Index");
        }

        [HttpPost]
        public IActionResult Index(PositionModel model)
        {
            if (ModelState.IsValid)
            {
                positions.Add(model);

                return View("CorrectionsOverview", positions);
            }
            ViewBag.ErrorMessage = "Failed to retrieve location data.";
            return View("Error");
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
