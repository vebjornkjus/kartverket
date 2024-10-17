using KartverketWebApp.API_Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace KartverketWebApp.Services
{
    public class SokeService : ISokeService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<SokeService> _logger;
        private readonly ApiSettings _apiSettings;

        public SokeService(HttpClient httpClient, ILogger<SokeService> logger, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiSettings = apiSettings.Value;

            _logger.LogInformation("SokeService initialized successfully.");
        }

        public async Task<KommunerResponse> GetSokeAsync(string kommuneName)
        {
            _logger.LogInformation("Sending API request for kommune: {KommuneName}", kommuneName);

            try
            {
                var response = await _httpClient.GetAsync($"{_apiSettings.KommuneInfoApiBaseUrl}/sok?utkoordsys=4258&knavn={kommuneName}");
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully received response from API for kommune: {KommuneName}", kommuneName);

                // Get the raw JSON content from the API response
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Raw JSON response: {Content}", content);

                var sokeResponse = JsonSerializer.Deserialize<KommunerResponse>(content);

                return sokeResponse;

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error occurred while calling API for kommune: {KommuneName}", kommuneName);
                throw; // Re-throw the exception to be handled by the controller
            }
        }
    }
}
