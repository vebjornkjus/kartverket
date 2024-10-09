// Services/LocationService.cs
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using KartverketWebApp.Models;
using Newtonsoft.Json;

namespace KartverketWebApp.Services
{
    public class StednavnService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StednavnService> _logger;

        public StednavnService(HttpClient httpClient, ILogger<StednavnService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<PositionModel> GetLocationData(double latitude, double longitude, int koordsys)
        {
            // Create the query string
            string query = $"?nord={latitude}&ost={longitude}&koordsys={koordsys}";

            // Define the API URL, assuming base URL is set up in configuration
            string url = "https://api.kartverket.no/kommuneinfo/v1/punkt" + query;

            // Send the HTTP GET request
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            // Ensure a successful response
            if (response.IsSuccessStatusCode)
            {
                // Parse the JSON response
                string jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PositionModel>(jsonString);
            }

            return null; // or handle error as needed
        }
    }
}

