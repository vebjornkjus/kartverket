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
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

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
public async Task<IActionResult> Index(int koordsys, string tittel, string beskrivelse, string mapType, string rapportType, List<KoordinatModel> koordinater, IFormFile? file)
{
    _logger.LogInformation($"Total coordinates received: {koordinater?.Count}");
    if (koordinater == null || !koordinater.Any())
    {
        _logger.LogWarning("Koordinater list is null or empty.");
        return View("Index");
    }

            if (ModelState.ContainsKey("file"))
            {
                ModelState["file"].Errors.Clear(); // Exclude file validation errors
            }

            if (ModelState.IsValid)
    {
        string filePath = null;

        // Handle file upload (optional)
        if (file != null && file.Length > 0)
        {
            try
            {
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                filePath = Path.Combine("uploads", uniqueFileName);
                var serverFilePath = Path.Combine("wwwroot", filePath);

                using (var stream = new FileStream(serverFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                _logger.LogInformation("File uploaded successfully: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading the file.");
            }
        }
        else
        {
            _logger.LogInformation("No file uploaded. Proceeding without a file.");
        }

        var firstKoord = koordinater.First();
        Steddata steddata = null;
        try
        {
            var stednavnResponse = await _stednavnService.GetStednavnAsync(firstKoord.Nord, firstKoord.Ost, koordsys);

            if (stednavnResponse != null)
            {
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
        }

        var newKart = new Kart
        {
            Koordsys = koordsys,
            Tittel = tittel,
            Beskrivelse = beskrivelse,
            MapType = mapType,
            RapportType = rapportType,
            SteddataId = steddata?.Id,
            FilePath = filePath
        };

        _context.Kart.Add(newKart);
        await _context.SaveChangesAsync();

        var newKoordinater = koordinater.Select((koord, index) => new Koordinater
        {
            KartEndringId = newKart.KartEndringId,
            Nord = koord.Nord,
            Ost = koord.Ost,
            Rekkefolge = index + 1
        }).ToList();

        _context.Koordinater.AddRange(newKoordinater);
        await _context.SaveChangesAsync();

        var tildelAnsattId = await GetTildelAnsattIdAsync(steddata?.Kommunenummer);

        var newRapport = new Rapport
        {
            RapportStatus = "Uapnet",
            Opprettet = DateTime.Now,
            KartEndringId = newKart.KartEndringId,
            PersonId = 1, // Temporary placeholder
            TildelAnsattId = tildelAnsattId,
            
        };

        _context.Rapport.Add(newRapport);
        await _context.SaveChangesAsync();

        return RedirectToAction("TakkRapport");
    }

    foreach (var modelState in ModelState.Values)
    {
        foreach (var error in modelState.Errors)
        {
            _logger.LogError(error.ErrorMessage);
        }
    }

    return View("Index");
}


        private async Task<int> GetTildelAnsattIdAsync(int? kommunenummer)
        {
            // Log the incoming kommunenummer
            _logger.LogInformation($"Finding Ansatt for Kommunenummer: {kommunenummer}");

            // Check for a match in Ansatt by Kommunenummer
            var matchingAnsatt = await _context.Ansatt
                .Where(a => a.Kommunenummer == kommunenummer)
                .OrderBy(a => _context.Rapport.Count(r => r.TildelAnsattId == a.AnsattId))
                .ThenBy(a => a.AnsattId)
                .FirstOrDefaultAsync();

            if (matchingAnsatt != null)
            {
                // Log the Ansatt details
                _logger.LogInformation($"Assigned to AnsattId: {matchingAnsatt.AnsattId}, Kommunenummer: {matchingAnsatt.Kommunenummer}");
                return matchingAnsatt.AnsattId;
            }

            // Log the fallback to AnsattId = 1
            _logger.LogWarning($"No matching Ansatt found for Kommunenummer: {kommunenummer}. Assigning to default AnsattId: 1.");
            return 1; // Default AnsattId
        }


        [HttpGet]
        public async Task<IActionResult> Saksbehandler(int ansattId = 1, int page = 1, int pageSize = 10)
        {
            // Fetch the associated `Ansatt` entity
            var ansatt = await _context.Ansatt.FirstOrDefaultAsync(a => a.AnsattId == ansattId);
            if (ansatt == null || string.IsNullOrEmpty(ansatt.Kommunenummer.ToString()))
            {
                _logger.LogWarning($"No valid Ansatt or Kommunenummer found for Ansatt ID: {ansattId}");
                return NotFound("Ansatt or Kommunenummer not found.");
            }

            // Fetch the associated `Person` entity
            var person = await _context.Person.FirstOrDefaultAsync(p => p.PersonId == ansatt.PersonId);
            if (person == null)
            {
                _logger.LogWarning($"No Person found for PersonId: {ansatt.PersonId}");
                return NotFound("Person not found.");
            }

            // Fetch the associated `Bruker` entity
            var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.BrukerId == person.BrukerId);
            string email = bruker?.Email ?? "Unknown";

            // Fetch polygon data
            string polygonCoordinates = null;
            try
            {
                var polygonResponse = await _httpClient.GetAsync($"{_apiSettings.KommuneInfoApiBaseUrl}/kommuner/{ansatt.Kommunenummer}/omrade");
                polygonResponse.EnsureSuccessStatusCode();

                var content = await polygonResponse.Content.ReadAsStringAsync();

                var jsonDoc = JsonDocument.Parse(content);
                if (jsonDoc.RootElement.TryGetProperty("omrade", out JsonElement omradeElement) &&
                    omradeElement.TryGetProperty("coordinates", out JsonElement coordinatesElement))
                {
                    polygonCoordinates = coordinatesElement.GetRawText();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching polygon data for kommunenummer: {Kommunenummer}", ansatt.Kommunenummer);
            }

            // Fetch reports assigned to the Ansatt
            var reports = await _context.Rapport
                .Where(r => r.TildelAnsattId == ansatt.AnsattId)
                .Include(r => r.Kart)
                    .ThenInclude(k => k.Koordinater)
                .OrderByDescending(r => r.Opprettet)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Extract markers from the reports
            var markers = reports
                .Where(r => r.Kart != null && r.Kart.Koordinater.Any())
                .Select(r => new
                {
                    RapportId = r.RapportId,
                    Nord = r.Kart.Koordinater.First().Nord,
                    Ost = r.Kart.Koordinater.First().Ost,
                    Tittel = r.Kart.Tittel
                }).ToList();

            // Serialize data to ViewBag
            ViewBag.MarkersJson = JsonSerializer.Serialize(markers);
            ViewBag.PolygonJson = polygonCoordinates;

            // Set pagination data
            var totalReports = await _context.Rapport.CountAsync(r => r.TildelAnsattId == ansatt.AnsattId);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalReports / (double)pageSize);

            // Pass user info to ViewBag
            ViewBag.UserName = person.Fornavn;
            ViewBag.UserLastName = person.Etternavn;
            ViewBag.UserEmail = email;

            // Prepare CombinedViewModel
            var combinedViewModel = new CombinedViewModel
            {
                Rapporter = reports
            };

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
