using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using Microsoft.Extensions.Logging;
using KartverketWebApp.Data;
using KartverketWebApp.API_Models;

namespace KartverketWebApp.Controllers
{
    public class RapportStatusController : Controller
    {
        // Private fields for dependencies
        private readonly ILogger<RapportStatusController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dbConnection;
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        // Constructor
        public RapportStatusController(
            ILogger<RapportStatusController> logger,
            ApplicationDbContext context,
            IDbConnection dbConnection,
            HttpClient httpClient,
            IOptions<ApiSettings> apiSettings)
        {
            _logger = logger;
            _context = context;
            _dbConnection = dbConnection;
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
        }

        [HttpPost]
        public IActionResult SetAvklart(int rapportId)
        {
            try
            {
                const string query = @"
                    UPDATE Rapport 
                    SET RapportStatus = 'Avklart',
                        BehandletDato = @BehandletDato
                    WHERE RapportId = @RapportId";

                var parameters = new
                {
                    RapportId = rapportId,
                    BehandletDato = DateTime.Now
                };

                _dbConnection.Execute(query, parameters);
                _logger.LogInformation($"Rapport {rapportId} er satt til Avklart");

                return RedirectToAction("RapportDetaljert", "Home", new { id = rapportId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Feil ved setting av rapport {rapportId} til Avklart: {ex.Message}");
                TempData["Error"] = "Det oppstod en feil ved oppdatering av rapporten.";
                return RedirectToAction("RapportDetaljert", "Home", new { id = rapportId });
            }
        }

        [HttpPost]
        public IActionResult SetFjernet(int rapportId)
        {
            try
            {
                const string query = @"
                    UPDATE Rapport 
                    SET RapportStatus = 'Fjernet',
                        BehandletDato = @BehandletDato
                    WHERE RapportId = @RapportId";

                var parameters = new
                {
                    RapportId = rapportId,
                    BehandletDato = DateTime.Now
                };

                _dbConnection.Execute(query, parameters);
                _logger.LogInformation($"Rapport {rapportId} er satt til Fjernet");

                return RedirectToAction("RapportDetaljert", "Home", new { id = rapportId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Feil ved setting av rapport {rapportId} til Fjernet: {ex.Message}");
                TempData["Error"] = "Det oppstod en feil ved oppdatering av rapporten.";
                return RedirectToAction("RapportDetaljert", "Home", new { id = rapportId });
            }
        }

        [HttpGet]
        public IActionResult HentTilgjengeligeAnsatte()
        {
            try
            {
                // Hent alle saksbehandlere (basert på BrukerType)
                const string query = @"
            SELECT a.AnsattId, 
                   CONCAT(p.Fornavn, ' ', p.Etternavn) as FulltNavn,
                   s.Kommunenavn
            FROM Ansatt a
            JOIN Person p ON a.PersonId = p.PersonId
            JOIN Bruker b ON p.BrukerId = b.BrukerId
            JOIN Steddata s ON a.Kommunenummer = s.Kommunenummer
            WHERE b.BrukerType = 'saksbehandler'
            ORDER BY p.Fornavn, p.Etternavn";

                var ansatte = _dbConnection.Query(query);
                return Json(ansatte);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Feil ved henting av tilgjengelige ansatte: {ex.Message}");
                return Json(new { error = "Kunne ikke hente liste over ansatte." });
            }
        }

        [HttpPost]
        public IActionResult TildelRapport(int rapportId, int nyAnsattId)
        {
            try
            {
                // Først sjekk om rapporten eksisterer og dens nåværende status
                const string checkRapportQuery = @"
            SELECT r.RapportStatus, r.TildelAnsattId, k.SteddataId
            FROM Rapport r
            JOIN Kart k ON r.KartEndringId = k.KartEndringId
            WHERE r.RapportId = @RapportId";

                var rapportInfo = _dbConnection.QueryFirstOrDefault(checkRapportQuery, new { RapportId = rapportId });

                if (rapportInfo == null)
                {
                    TempData["Error"] = "Rapporten ble ikke funnet.";
                    return RedirectToAction("RapportDetaljert", "Home", new { id = rapportId });
                }

                // Sjekk om den nye ansatte er en gyldig saksbehandler for dette området
                const string checkAnsattQuery = @"
            SELECT a.AnsattId, s.Kommunenummer
            FROM Ansatt a
            JOIN Person p ON a.PersonId = p.PersonId
            JOIN Bruker b ON p.BrukerId = b.BrukerId
            JOIN Steddata s ON a.Kommunenummer = s.Kommunenummer
            WHERE a.AnsattId = @AnsattId 
            AND b.BrukerType = 'saksbehandler'";

                var ansattInfo = _dbConnection.QueryFirstOrDefault(checkAnsattQuery, new { AnsattId = nyAnsattId });

                if (ansattInfo == null)
                {
                    TempData["Error"] = "Ugyldig saksbehandler valgt.";
                    return RedirectToAction("RapportDetaljert", "Home", new { id = rapportId });
                }

                // Oppdater rapporten med ny saksbehandler
                const string updateQuery = @"
            UPDATE Rapport 
            SET TildelAnsattId = @NyAnsattId,
                RapportStatus = CASE 
                    WHEN RapportStatus = 'Uåpnet' THEN 'Under behandling'
                    ELSE RapportStatus 
                END
            WHERE RapportId = @RapportId";

                var parameters = new
                {
                    RapportId = rapportId,
                    NyAnsattId = nyAnsattId
                };

                _dbConnection.Execute(updateQuery, parameters);

                // Logg endringen
                _logger.LogInformation($"Rapport {rapportId} er tildelt ansatt {nyAnsattId}");
                TempData["Success"] = "Rapporten ble overført til ny saksbehandler.";

                return RedirectToAction("RapportDetaljert", "Home", new { id = rapportId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Feil ved tildeling av rapport {rapportId} til ansatt {nyAnsattId}: {ex.Message}");
                TempData["Error"] = "Det oppstod en feil ved tildeling av rapporten.";
                return RedirectToAction("RapportDetaljert", "Home", new { id = rapportId });
            }
        }
    }
}