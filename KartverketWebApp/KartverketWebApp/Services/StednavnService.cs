using KartverketWebApp.API_Models;
using KartverketWebApp.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using System.Web;

namespace KartverketWebApp.Services
{
        public class StednavnService : IStednavn
    {
            private readonly HttpClient _httpClient;
            private readonly ILogger<StednavnService> _logger;
            private readonly ApiSettings _apiSettings;
        public StednavnService(HttpClient httpClient, ILogger<StednavnService> logger, IOptions<ApiSettings> apiSettings)
            {
                _httpClient = httpClient;
                _logger = logger;
                _apiSettings = apiSettings.Value;

            _logger.LogInformation("StednavnService initialized successfully.");
            }
        public async Task<StednavnResponse> GetStednavnAsync(double nord, double ost, int koordsys)
        {
            _logger.LogInformation("GetStednavnAsync method called.");

            try
            {
                _logger.LogInformation("Reached before API request log.");
                _logger.LogInformation($"Sending API request to: {_apiSettings.StedsnavnApiBaseUrl}/punkt?nord={nord}&ost={ost}&koordsys={koordsys}");
                _logger.LogInformation("Reached after API request log.");

                var response = await _httpClient.GetAsync($"{_apiSettings.StedsnavnApiBaseUrl}/punkt?nord={nord}&ost={ost}&koordsys={koordsys}");
                response.EnsureSuccessStatusCode();


                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Position Data Response: {json}");


                var stednavnResponse = JsonSerializer.Deserialize<StednavnResponse>(json);
                // Check if data was deserialized properly
                if (stednavnResponse != null)
                {
                    _logger.LogInformation($"Deserialized StednavnResponse: Fylkesnavn = {stednavnResponse.Fylkesnavn}, Kommunenavn = {stednavnResponse.Kommunenavn}, Kommunenummer = {stednavnResponse.Kommunenummer}");
                }
                else
                {
                    _logger.LogWarning("StednavnResponse is null after deserialization.");
                }

                return stednavnResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching position data: {ex.Message}");
                return new StednavnResponse { Fylkesnavn = "Error: Unable to fetch data" };
            }
        }
    }
    }

