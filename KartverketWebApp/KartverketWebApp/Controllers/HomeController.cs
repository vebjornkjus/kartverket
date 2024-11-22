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
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace KartverketWebApp.Controllers
{
    public class HomeController : Controller
    {
        // Private fields for dependencies
        private readonly ILogger<HomeController> _logger;
        private readonly IStednavn _stednavnService;
        private readonly ISokeService _sokeService;
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dbConnection;

        // Static lists for temporary data storage
        private static List<PositionModel> positions = new List<PositionModel>();
        private static List<StednavnViewModel> stednavn = new List<StednavnViewModel>();

        // Constructor
        public HomeController(
            ILogger<HomeController> logger,
            IStednavn stedsnavnService,
            ISokeService sokeService,
            HttpClient httpClient,
            IOptions<ApiSettings> apiSettings,
            ApplicationDbContext context,
            IDbConnection dbConnection)
        {
            _logger = logger;
            _stednavnService = stedsnavnService;
            _sokeService = sokeService;
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _context = context;
            _dbConnection = dbConnection;
        }

        // SECTION: Static Views
        public IActionResult TakkRapport() => View("~/Views/Home/Innsender/TakkRapport.cshtml");
        public IActionResult Innlogging() => View();
        public IActionResult Admin() => View();
        public IActionResult soke() => View();
        public IActionResult Privacy() => View();

        // SECTION: CRUD Operations
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

                // Håndterer filopplastning
                if (file != null && file.Length > 0)
                {
                    try
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        filePath = Path.Combine("RapportBilder", uniqueFileName);
                        var serverFilePath = Path.Combine("wwwroot", filePath);

                        using (var stream = new FileStream(serverFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        _logger.LogInformation("File opplastning vellykket: {FilePath}", filePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Et problem oppsto ved opplastningen.");
                    }
                }
                else
                {
                    _logger.LogInformation("Ingen fil lastet opp. Fortsetter uten en fil.");
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

                // Legg til logikk for å hente innlogget brukers BrukerId
                var brukerId = User.FindFirstValue(ClaimTypes.Name); // Henter brukerens e-post fra claims
                var bruker = _context.Bruker.FirstOrDefault(b => b.Email == brukerId);
                if (bruker == null)
                {
                    _logger.LogWarning("Bruker ikke funnet.");
                    return BadRequest("Bruker ikke funnet.");
                }

                var person = _context.Person.FirstOrDefault(p => p.BrukerId == bruker.BrukerId);
                if (person == null)
                {
                    _logger.LogWarning("Person ikke funnet.");
                    return BadRequest("Person ikke funnet.");
                }

                var newRapport = new Rapport
                {
                    RapportStatus = "Uåpnet",
                    Opprettet = DateTime.Now,
                    KartEndringId = newKart.KartEndringId,
                    PersonId = person.PersonId, // Koble rapport til innlogget person
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

        [HttpPost]
        public IActionResult UpdateStatusAndRedirect(int id)
        {
            // SQL query to update status
            string query = @"
        UPDATE Rapport
        SET RapportStatus = 'Under behandling'
        WHERE RapportId = @RapportId AND RapportStatus = 'Uåpnet';
    ";

            // Execute the query
            var rowsAffected = _dbConnection.Execute(query, new { RapportId = id });

            // Redirect to the detailed view, regardless of whether the status was updated
            return RedirectToAction("RapportDetaljert", "Home", new { id });
        }

        // SECTION: Fetching and Processing Reports
        [Authorize(Policy = "AdminOrSaksbehandlerPolicy")]
        [HttpGet]
        public async Task<IActionResult> Saksbehandler(int ansattId = 1, int activePage = 1, int resolvedPage = 1, int pageSize = 10)
        {
            // Fetch the logged-in user's email
            var userEmail = User.FindFirstValue(ClaimTypes.Name); // Retrieve the logged-in user's email
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("User email not found in claims.");
                return Unauthorized("User not logged in.");
            }

            _logger.LogInformation($"Fetching data for user email: {userEmail}");

            // Find the user in the database
            var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.Email == userEmail);
            if (bruker == null)
            {
                _logger.LogWarning($"No Bruker found for email: {userEmail}");
                return NotFound("User not found.");
            }

            // Fetch the associated Person and Ansatt entities
            var person = await _context.Person.FirstOrDefaultAsync(p => p.BrukerId == bruker.BrukerId);
            if (person == null)
            {
                _logger.LogWarning($"No Person found for BrukerId: {bruker.BrukerId}");
                return NotFound("Person not found.");
            }

            var ansatt = await _context.Ansatt.FirstOrDefaultAsync(a => a.PersonId == person.PersonId);
            if (ansatt == null || ansatt.Kommunenummer == 0)
            {
                _logger.LogWarning($"Ansatt not found or invalid kommunenummer for PersonId: {person.PersonId}");
                return NotFound("Ansatt or kommunenummer not found.");
            }

            // Add user information to ViewBag
            ViewBag.UserName = person.Fornavn;
            ViewBag.UserLastName = person.Etternavn;
            ViewBag.UserEmail = userEmail;

            // Fetch polygon data
            string polygonCoordinates = null;
            if (ansatt.Kommunenummer == 0)
            {
                _logger.LogWarning("Invalid kommunenummer: 0");
                polygonCoordinates = "[]"; // Fallback to empty polygons
            }
            else
            {
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
            }

            // Fetch total counts for active and resolved reports
            var totalActiveReports = await _context.Rapport
                .CountAsync(r => r.TildelAnsattId == ansatt.AnsattId &&
                                 (r.RapportStatus == "Uåpnet" || r.RapportStatus == "Under behandling"));

            var totalResolvedReports = await _context.Rapport
                .CountAsync(r => r.TildelAnsattId == ansatt.AnsattId &&
                                 (r.RapportStatus == "Avklart" || r.RapportStatus == "Avvist"));

            // Fetch paginated active and resolved reports
            var activeReports = await _context.Rapport
                .Where(r => r.TildelAnsattId == ansatt.AnsattId &&
                            (r.RapportStatus == "Uåpnet" || r.RapportStatus == "Under behandling"))
                .Include(r => r.Kart)
                    .ThenInclude(k => k.Koordinater)
                .OrderByDescending(r => r.Opprettet)
                .Skip((activePage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var resolvedReports = await _context.Rapport
                .Where(r => r.TildelAnsattId == ansatt.AnsattId &&
                            (r.RapportStatus == "Avklart" || r.RapportStatus == "Avvist"))
                .Include(r => r.Kart)
                    .ThenInclude(k => k.Koordinater)
                .OrderByDescending(r => r.Opprettet)
                .Skip((resolvedPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Prepare markers for the view
            var markers = activeReports
                .Where(r => r.Kart != null && r.Kart.Koordinater.Any())
                .Select(r => new
                {
                    RapportId = r.RapportId,
                    Nord = r.Kart.Koordinater.First().Nord,
                    Ost = r.Kart.Koordinater.First().Ost,
                    Tittel = r.Kart.Tittel
                }).ToList();

            // Assign raw polygon JSON directly to the ViewBag
            ViewBag.PolygonJson = polygonCoordinates; // Use raw string, not serialized again
            ViewBag.MarkersJson = JsonSerializer.Serialize(markers); // Serialize markers
            ViewBag.ActiveCurrentPage = activePage;
            ViewBag.ActiveTotalPages = (int)Math.Ceiling(totalActiveReports / (double)pageSize);
            ViewBag.ResolvedCurrentPage = resolvedPage;
            ViewBag.ResolvedTotalPages = (int)Math.Ceiling(totalResolvedReports / (double)pageSize);
            // Prepare the combined view model
            var combinedViewModel = new CombinedViewModel
            {
                ActiveRapporter = activeReports,
                ResolvedRapporter = resolvedReports
            };

            return View("~/Views/Home/Saksbehandler/Saksbehandler.cshtml", combinedViewModel);
        }

        [Authorize(Policy = "AdminOrSaksbehandlerPolicy")]
        [HttpGet]
        public async Task<IActionResult> MineRapporter(int activePage = 1, int pageSize = 10)
        {
            // Fetch the logged-in user's email
            var userEmail = User.FindFirstValue(ClaimTypes.Name); // Retrieve the logged-in user's email
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("User email not found in claims.");
                return Unauthorized("User not logged in.");
            }

            _logger.LogInformation($"Fetching data for user email: {userEmail}");

            // Find the user in the database
            var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.Email == userEmail);
            if (bruker == null)
            {
                _logger.LogWarning($"No Bruker found for email: {userEmail}");
                return NotFound("User not found.");
            }

            // Fetch the associated Person and Ansatt entities
            var person = await _context.Person.FirstOrDefaultAsync(p => p.BrukerId == bruker.BrukerId);
            if (person == null)
            {
                _logger.LogWarning($"No Person found for BrukerId: {bruker.BrukerId}");
                return NotFound("Person not found.");
            }

            var ansatt = await _context.Ansatt.FirstOrDefaultAsync(a => a.PersonId == person.PersonId);
            if (ansatt == null || ansatt.Kommunenummer == 0)
            {
                _logger.LogWarning($"Ansatt not found or invalid kommunenummer for PersonId: {person.PersonId}");
                return NotFound("Ansatt or kommunenummer not found.");
            }

            // Add user information to ViewBag
            ViewBag.UserName = person.Fornavn;
            ViewBag.UserLastName = person.Etternavn;
            ViewBag.UserEmail = userEmail;

            // Fetch only active reports
            var activeReports = await _context.Rapport
                .Where(r => r.TildelAnsattId == ansatt.AnsattId &&
                            (r.RapportStatus == "Uåpnet" || r.RapportStatus == "Under behandling"))
                .Include(r => r.Kart) // Include Kart entity
                .OrderByDescending(r => r.Opprettet)
                .Skip((activePage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Populate the CombinedViewModel
            var combinedViewModel = new CombinedViewModel
            {
                ActiveRapporter = activeReports
            };

            ViewBag.ActiveCurrentPage = activePage;
            ViewBag.ActiveTotalPages = (int)Math.Ceiling((double)activeReports.Count / pageSize);

            return View("~/Views/Home/Saksbehandler/MineRapporter.cshtml", combinedViewModel);
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

        // SECTION: Search Functionality
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
[Authorize] // Sørger for at kun innloggede brukere kan få tilgang
public async Task<IActionResult> MinSide()
{
    // Hent e-post fra den innloggede brukeren
    var email = User.Identity?.Name;

    if (string.IsNullOrEmpty(email))
    {
        _logger.LogWarning("Ingen bruker er logget inn.");
        return RedirectToAction("Login", "Account");
    }

    // Hent bruker fra databasen basert på e-post
    var bruker = await _context.Bruker
        .Include(b => b.Personer) // Henter relaterte personer
        .FirstOrDefaultAsync(b => b.Email == email);

    if (bruker == null)
    {
        _logger.LogWarning($"Bruker med e-post {email} ble ikke funnet.");
        return RedirectToAction("Login", "Account");
    }

    // Hent den første tilknyttede personen
    var person = bruker.Personer.FirstOrDefault();
    if (person == null)
    {
        _logger.LogWarning($"Ingen personer er tilknyttet brukeren med e-post {email}.");
        return RedirectToAction("Login", "Account");
    }

    // Hent rapporter knyttet til personen
    var rapporter = await _context.Rapport
        .Include(r => r.Kart) // Henter relasjoner til Kart
        .Where(r => r.PersonId == person.PersonId) // Filtrer rapportene for denne personen
        .ToListAsync();

    // Opprett ViewModel for å sende data til view
    var viewModel = new MinSideViewModel
    {
        Bruker = bruker,
        Person = person,
        Rapporter = rapporter
    };

    return View(viewModel);
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


        // SECTION: Helper Methods
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
    }
}

