using KartverketWebApp.API_Models;
using KartverketWebApp.Models;
using KartverketWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using KartverketWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static KartverketWebApp.Models.PositionModel;

namespace KartverketWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStednavn _stednavnService;
        private readonly ISokeService _sokeService;
        private static List<PositionModel> positions = new List<PositionModel>();
        private static List<StednavnViewModel> stednavn = new List<StednavnViewModel>();
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, IStednavn stedsnavnService, ISokeService sokeService, HttpClient httpClient, IOptions<ApiSettings> apiSettings, ApplicationDbContext context)
        {
            _logger = logger;
            _stednavnService = stedsnavnService;
            _sokeService = sokeService;
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _context = context;
        }

        public IActionResult TakkRapport()
        {
            return View();
        }

        public IActionResult Innlogging()
        {
            return View();
        }

        public IActionResult Admin()
        {
            return View();
        }

        public IActionResult soke()
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
        public async Task<IActionResult> Index(int koordsys, string tittel, string beskrivelse, string mapType, string rapportType, List<KoordinatModel> koordinater)
        {
            _logger.LogInformation($"Total coordinates received: {koordinater?.Count}");
            foreach (var koord in koordinater)
            {
                _logger.LogInformation($"Nord: {koord.Nord}, Ost: {koord.Ost}");
            }
            if (ModelState.IsValid)
            {
                // Add the Kart to the database
                var newKart = new Kart
                {
                    Koordsys = koordsys,
                    Tittel = tittel,
                    Beskrivelse = beskrivelse,
                    MapType = mapType,
                    RapportType = rapportType
                };
               
                _context.Kart.Add(newKart);
                await _context.SaveChangesAsync();

                var newKoordinater = koordinater.Select((koord, index) => new Koordinater
                {
                    KartEndringId = newKart.KartEndringId,
                    Nord = koord.Nord,
                    Ost = koord.Ost,
                    Rekkefolge = index + 1  // Order in sequence
                }).ToList();

                _logger.LogInformation("Received coordinates:");
                foreach (var koord in koordinater)
                {
                    _logger.LogInformation($"Nord: {koord.Nord}, Ost: {koord.Ost}");
                }

                _context.Koordinater.AddRange(newKoordinater);
                await _context.SaveChangesAsync();

                var newRapport = new Rapport
                {
                    RapportStatus = "Uåpnet",
                    Opprettet = DateTime.Now,
                    KartEndringId = newKart.KartEndringId, // Associate the newRapport with the newKart
                    PersonId = 1 //Setter person id som 1 MIDLERTIDIG
                };

                _context.Rapport.Add(newRapport);
                await _context.SaveChangesAsync();

                // Redirect to the TakkRapport action to display the updated list
                return RedirectToAction("TakkRapport");
            }
            
            return View("Error");
        }

        public async Task<IActionResult> Saksbehandler()
        {
            var rapporter = await _context.Rapport
                .Include(r => r.Person)
                .Include(r => r.Kart)
                    .ThenInclude(k => k.Koordinater)
                .ToListAsync();

            var stednavnList = new List<StednavnViewModel>();

            foreach (var rapport in rapporter)
            {
                var kart = rapport.Kart;

                if (kart?.Koordinater != null && kart.Koordinater.Count > 0)
                {
                    var firstCoordinate = kart.Koordinater.First();
                    var stednavnResponse = await _stednavnService.GetStednavnAsync(firstCoordinate.Nord, firstCoordinate.Ost, kart.Koordsys);

                    if (stednavnResponse != null)
                    {
                        stednavnList.Add(new StednavnViewModel
                        {
                            Fylkesnavn = stednavnResponse.Fylkesnavn,
                            Fylkesnummer = stednavnResponse.Fylkesnummer,
                            Kommunenavn = stednavnResponse.Kommunenavn,
                            Kommunenummer = stednavnResponse.Kommunenummer,
                            KartEndringId = kart.KartEndringId  // Link this Stednavn to the specific Kart
                        });
                    }
                }
                else
                {
                    _logger.LogWarning("Kart {KartId} has no coordinates.", kart?.KartEndringId);
                }
            }

            var combinedViewModel = new CombinedViewModel
            {
                Rapporter = rapporter,
                Stednavn = stednavnList
            };

            return View(combinedViewModel);
        }



        public IActionResult CorrectionsOverview()
        {
            // Prepare the CombinedViewModel to be passed to the view
            var kartData = _context.Kart.ToList();
            var koordinatData = _context.Koordinater.ToList();

            var viewModel = new CombinedViewModel
            {
                Positions = positions ?? new List<PositionModel>(), // Ensure positions is not null
                Stednavn = stednavn ?? new List<StednavnViewModel>(), // Initialize as needed
                KartData = _context.Kart
                    .Include(k => k.Koordinater) // Laster inn relasjon til Koordinater
                    .Include(k => k.Rapporter)   // Laster inn relasjon til Rapporter
                    .ToList(),
                KoordinatData = _context.Koordinater
                    .Include(k => k.Kart) // Laster inn relasjon til Kart
                    .ToList()
            };

            foreach (var navn in viewModel.Stednavn)
            {
                _logger.LogInformation($"Stednavn data: Fylkesnavn = {navn.Fylkesnavn}, Kommunenavn = {navn.Kommunenavn}");
            }

            return View(viewModel); // Return the view with CombinedViewModel
        }

        [HttpPost]
        public async Task<IActionResult> Sok(string kommuneName)
        {
            if (string.IsNullOrEmpty(kommuneName))
            {
                ViewData["Error"] = "Please enter a valid Kommune Number.";
                return View("Index");
            }

            try
            {
                // Get the full KommunerResponse
                var kommunerResponse = await _sokeService.GetSokeAsync(kommuneName);

                if (kommunerResponse != null && kommunerResponse.AntallTreff > 0)
                {
                    // Ensure that we access the first municipality in the list
                    var kommune = kommunerResponse.Kommuner.FirstOrDefault();

                    if (kommune != null)
                    {
                        _logger.LogInformation("Received valid response from API.");
                        _logger.LogInformation($"Komunenummer: {kommune.Kommunenummer}");

                        var viewModel = new SokeModel
                        {
                            Kommunenummer = kommune.Kommunenummer ?? "N/A",
                        };

                        return RedirectToAction("Soke", new { kommunenummer = viewModel.Kommunenummer });
                    }
                }

                // If no results were found
                ViewData["Error"] = $"No results found for Kommune '{kommuneName}'.";
                return View("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during API call to GetSokeAsync");
                ViewData["Error"] = "An error occurred while fetching data. Please try again.";
                return View("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Soke(string kommunenummer)
        {
            _logger.LogInformation($"Komunenummer passed from sokeModel: {kommunenummer}");

            // Fetch the raw JSON from the API
            var response = await _httpClient.GetAsync($"{_apiSettings.KommuneInfoApiBaseUrl}/kommuner/{kommunenummer}/omrade");
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Successfully received response from API for kommune: {kommunenummer}", kommunenummer);

            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Raw JSON response: {Content}", content);

            try
            {
                // Parse the JSON content (without deserializing it into a C# class)
                var jsonDoc = JsonDocument.Parse(content);

                // Check if 'omrade' and 'coordinates' properties exist
                if (jsonDoc.RootElement.TryGetProperty("omrade", out JsonElement omradeElement) &&
                    omradeElement.TryGetProperty("coordinates", out JsonElement coordinatesElement))
                {
                    // Extract the 'coordinates' array from the 'omrade' object
                    var coordinates = coordinatesElement.GetRawText();

                    // Pass the raw coordinates to the view using ViewBag
                    ViewBag.Coordinates = coordinates;
                    _logger.LogInformation("API response for coordinates: {coordinates}", coordinates);

                    return View("Soke");
                }
                else
                {
                    _logger.LogError("The expected properties 'omrade' or 'coordinates' were not found in the JSON response.");
                    return BadRequest("Invalid JSON structure.");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing JSON for kommune: {kommunenummer}", kommunenummer);
                return BadRequest("Invalid JSON format.");
            }
        }

    }
}
