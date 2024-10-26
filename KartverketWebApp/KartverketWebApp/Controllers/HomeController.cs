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


        public HomeController(ILogger<HomeController> logger, IStednavn stedsnavnService, ISokeService sokeService, HttpClient httpClient, IOptions<ApiSettings> apiSettings)
        {
            _logger = logger;
            _stednavnService = stedsnavnService;
            _sokeService = sokeService;
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        public IActionResult TakkRapport()
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
        public async Task<IActionResult> Index(PositionModel positionModel, StednavnViewModel stednavnModel)
        {
            if (ModelState.IsValid)
            {
                var newPosition = new PositionModel
                {
                    Kart_endring_id = Guid.NewGuid().ToString(),
                    Koordsys = positionModel.Koordsys,
                    Tittel = positionModel.Tittel,
                    Description = positionModel.Description,
                    Map_type = positionModel.Map_type,
                    Rapport_type = positionModel.Rapport_type
                };

                // Add each coordinate from the submitted PositionModel to the newPositionModel's Coordinates list
                foreach (var coordinate in positionModel.Coordinates)
                {
                    newPosition.Coordinates.Add(new PositionModel.Coordinate
                    {
                        Nord = coordinate.Nord,
                        Ost = coordinate.Ost
                    });
                    _logger.LogInformation("Added coordinate: Nord = {Nord}, Ost = {Ost}", coordinate.Nord, coordinate.Ost);
                }

                positions.Add(newPosition);

                if (positionModel.Coordinates.Count > 0)
                {
                    var firstCoordinate = positionModel.Coordinates[0];

                    // Call the Stednavn API using the first coordinate
                    var stednavnResponse = await _stednavnService.GetStednavnAsync(firstCoordinate.Nord, firstCoordinate.Ost, positionModel.Koordsys);


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
                        ViewData["Error"] = $"No results found for coordinates: nord {firstCoordinate.Nord}, ost {firstCoordinate.Ost}, Koordsys {positionModel.Koordsys}.";
                        return View("Index");
                    }
                }

                _logger.LogError("Failed to retrieve location data for PositionModel: Coordinates = {Coordinates}, Koordsys = {Koordsys}", positionModel.Coordinates, positionModel.Koordsys);

            }
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
