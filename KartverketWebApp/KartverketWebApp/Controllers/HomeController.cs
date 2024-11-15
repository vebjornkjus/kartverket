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
            return View("~/Views/Home/Innsender/TakkRapport.cshtml");
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
        [HttpPost]
        public async Task<IActionResult> Index(int koordsys, string tittel, string beskrivelse, string mapType, string rapportType, List<KoordinatModel> koordinater)
        {
            _logger.LogInformation($"Total coordinates received: {koordinater?.Count}");
            if (koordinater == null || !koordinater.Any())
            {
                _logger.LogWarning("Koordinater list is null or empty.");
                return View("Index");
            }

            if (ModelState.IsValid)
            {
                // Fetch Stednavn data using the first coordinate
                var firstKoord = koordinater.First();
                Steddata steddata = null;
                try
                {
                    // Call the service to get Stednavn data
                    var stednavnResponse = await _stednavnService.GetStednavnAsync(firstKoord.Nord, firstKoord.Ost, koordsys);

                    if (stednavnResponse != null)
                    {
                        // Parse Fylkesnummer and Kommunenummer
                        int fylkesnummer = 0;
                        if (!string.IsNullOrEmpty(stednavnResponse.Fylkesnummer))
                        {
                            int.TryParse(stednavnResponse.Fylkesnummer, out fylkesnummer);
                        }

                        int kommunenummer = 0;
                        if (!string.IsNullOrEmpty(stednavnResponse.Kommunenummer))
                        {
                            int.TryParse(stednavnResponse.Kommunenummer, out kommunenummer);
                        }

                        // Create and save Steddata first
                        steddata = new Steddata
                        {
                            Fylkenavn = stednavnResponse.Fylkesnavn ?? "N/A",
                            Kommunenavn = stednavnResponse.Kommunenavn ?? "N/A",
                            Fylkenummer = fylkesnummer,
                            Kommunenummer = kommunenummer
                        };

                        _context.Steddata.Add(steddata);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching Stednavn data.");
                    // Handle exceptions as needed
                }

                // Now create and save the Kart with SteddataId
                var newKart = new Kart
                {
                    Koordsys = koordsys,
                    Tittel = tittel,
                    Beskrivelse = beskrivelse,
                    MapType = mapType,
                    RapportType = rapportType,
                    SteddataId = steddata?.Id, // This can be null if Steddata is null
                };

                _context.Kart.Add(newKart);
                await _context.SaveChangesAsync();

                // Add Koordinater to the database
                var newKoordinater = koordinater.Select((koord, index) => new Koordinater
                {
                    KartEndringId = newKart.KartEndringId,
                    Nord = koord.Nord,
                    Ost = koord.Ost,
                    Rekkefolge = index + 1
                }).ToList();

                _context.Koordinater.AddRange(newKoordinater);
                await _context.SaveChangesAsync();

                // Add the Rapport to the database
                var newRapport = new Rapport
                {
                    RapportStatus = "Uåpnet",
                    Opprettet = DateTime.Now,
                    KartEndringId = newKart.KartEndringId,
                    PersonId = 1, // Temporary placeholder
                    TildelAnsattId = 1
                };

                _context.Rapport.Add(newRapport);
                await _context.SaveChangesAsync();

                // Redirect to the TakkRapport action
                return RedirectToAction("TakkRapport");
            }

            // Log ModelState errors here before returning the view
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogError(error.ErrorMessage);
                }
            }

            return View("Index");
        }

        public async Task<IActionResult> Saksbehandler(int page = 1, int pageSize = 10)
        {
            // Calculate the total number of reports for pagination
            var totalReports = await _context.Rapport.CountAsync();

            // Fetch paginated data
            var paginatedReports = await _context.Rapport
                .Include(r => r.Person)
                .Include(r => r.Kart)
                    .ThenInclude(k => k.Koordinater)
                .Include(r => r.Kart)
                    .ThenInclude(k => k.Steddata) // Include Steddata directly with Kart
                .OrderByDescending(r => r.Opprettet) // Optional: Sort by creation date or another relevant field
                .Skip((page - 1) * pageSize) // Skip rows for previous pages
                .Take(pageSize) // Take rows for the current page
                .ToListAsync();

            // Create CombinedViewModel with paginated data
            var combinedViewModel = new CombinedViewModel
            {
                Rapporter = paginatedReports
            };

            // Add pagination metadata
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalReports / (double)pageSize);

            return View("~/Views/Home/Saksbehandler/Saksbehandler.cshtml", combinedViewModel);
        }



        [HttpGet]
        public async Task<IActionResult> RapportDetaljert(int id)
        {
            var rapport = await _context.Rapport
                .Include(r => r.Kart)
                    .ThenInclude(k => k.Koordinater)
                .Include(r => r.Person)
                    .ThenInclude(p => p.Bruker) // Include Bruker related to the Person
                .FirstOrDefaultAsync(r => r.RapportId == id);

            if (rapport == null)
            {
                return NotFound();
            }

            var kart = rapport.Kart;
            Steddata steddata = null;

            // If Kart and its Koordinater exist, retrieve the first coordinate and then fetch Steddata
            if (kart?.Koordinater?.Any() == true)
            {
                var firstCoord = kart.Koordinater.First();
                steddata = await _context.Steddata
                    .FirstOrDefaultAsync(s => s.Id == kart.SteddataId);

                if (steddata == null)
                {
                    _logger.LogWarning("Steddata for Kart {KartId} was not found.", kart.KartEndringId);
                }
            }

            var viewModel = new DetaljertViewModel
            {
                Rapport = rapport,
                Kart = rapport.Kart,
                Person = rapport.Person,
                Bruker = rapport.Person?.Bruker,
                Steddata = steddata
            };

            return View("~/Views/Home/Saksbehandler/RapportDetaljert.cshtml", viewModel);
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

        public PartialViewResult Oversikt()
        {
            return PartialView("_Oversikt");
        }

        public PartialViewResult MineRapporter()
        {
            return PartialView("_MineRapporter");
        }


        public PartialViewResult Varslinger()
        {
            return PartialView("_Oversikt");
        }

        public PartialViewResult Meldinger()
        {
            return PartialView("_Oversikt");
        }
        
        public PartialViewResult TidligereRapporter()
        {
            return PartialView("_Oversikt");
        }



    }
}
