using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using Microsoft.Extensions.Logging;
using KartverketWebApp.Data;
using KartverketWebApp.API_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace KartverketWebApp.Controllers
{
    [Authorize(Policy = "AdminOrSaksbehandlerPolicy")]
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
        [Authorize]
        [HttpGet]

        public IActionResult HentTilgjengeligeAnsatte()
        {
            try
            {
                // Først hent kommunenummer for innlogget bruker
                const string userQuery = @"
            SELECT DISTINCT a.Kommunenummer
            FROM Ansatt a
            INNER JOIN Person p ON a.PersonId = p.PersonId
            INNER JOIN Bruker b ON p.BrukerId = b.BrukerId
            WHERE b.Email = @UserEmail";  // Anta at vi bruker email for å identifisere brukeren

                var userEmail = User.Identity.Name; // Henter innlogget brukers email
                var userKommunenummer = _dbConnection.QueryFirstOrDefault<int>(userQuery, new { UserEmail = userEmail });

                // Så hent alle ansatte fra samme kommune
                const string query = @"
            SELECT 
                a.AnsattId as AnsattId,
                p.Fornavn as Fornavn,
                p.Etternavn as Etternavn,
                s.Kommunenavn as Kommunenavn
            FROM Ansatt a
            INNER JOIN Person p ON a.PersonId = p.PersonId
            INNER JOIN Bruker b ON p.BrukerId = b.BrukerId
            LEFT JOIN Steddata s ON a.Kommunenummer = s.Kommunenummer
            WHERE b.BrukerType = 'saksbehandler'
            AND a.Kommunenummer = @Kommunenummer
            AND b.Email != @CurrentUserEmail  -- Ekskluder innlogget bruker
            ORDER BY p.Fornavn, p.Etternavn";

                var ansatte = _dbConnection.Query(query, new
                {
                    Kommunenummer = userKommunenummer,
                    CurrentUserEmail = userEmail
                });

                return Json(ansatte);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Feil ved henting av tilgjengelige ansatte: {ex.Message}");
                return Json(new { error = "Kunne ikke hente liste over ansatte." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TildelRapport([FromBody] TildelRapportModel model)
        {
            try
            {
                const string query = @"
            UPDATE Rapport 
            SET TildelAnsattId = @NyAnsattId,
                RapportStatus = CASE 
                    WHEN RapportStatus = 'Uåpnet' THEN 'Under behandling'
                    ELSE RapportStatus 
                END
            WHERE RapportId = @RapportId";

                _dbConnection.Execute(query, new
                {
                    RapportId = model.RapportId,
                    NyAnsattId = model.NyAnsattId
                });

                // Since this is an AJAX call, we'll return JSON with a redirect URL
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Saksbehandler", "Home")  // Adjust controller name if needed
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Feil ved tildeling av rapport {model.RapportId} til ansatt {model.NyAnsattId}: {ex.Message}");
                return Json(new { success = false, error = "Kunne ikke tildele rapport." });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromForm] string innhold, [FromForm] int rapportId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(innhold))
                {
                    return Json(new { success = false, message = "Comment content cannot be empty." });
                }

                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Json(new { success = false, message = "User not authenticated properly." });
                }

                // Get sender's PersonId using Dapper
                const string senderQuery = @"
            SELECT p.PersonId
            FROM Person p
            INNER JOIN Bruker b ON p.BrukerId = b.BrukerId
            WHERE b.Email = @Email";

                var senderPersonId = await _dbConnection.QueryFirstOrDefaultAsync<int?>(senderQuery, new { Email = userEmail });

                if (!senderPersonId.HasValue)
                {
                    return Json(new { success = false, message = "User profile not found." });
                }

                // Get recipient's PersonId using Dapper
                const string recipientQuery = @"
            SELECT PersonId
            FROM Rapport
            WHERE RapportId = @RapportId";

                var mottakerPersonId = await _dbConnection.QueryFirstOrDefaultAsync<int?>(recipientQuery, new { RapportId = rapportId });

                if (!mottakerPersonId.HasValue)
                {
                    return Json(new { success = false, message = "Rapport not found." });
                }

                // Insert new message using Dapper
                const string insertQuery = @"
            INSERT INTO Meldinger (RapportId, SenderPersonId, MottakerPersonId, Innhold, Tidsstempel, Status)
            VALUES (@RapportId, @SenderPersonId, @MottakerPersonId, @Innhold, @Tidsstempel, @Status)";

                var parameters = new
                {
                    RapportId = rapportId,
                    SenderPersonId = senderPersonId.Value,
                    MottakerPersonId = mottakerPersonId.Value,
                    Innhold = innhold,
                    Tidsstempel = DateTime.Now,
                    Status = "sendt"
                };

                await _dbConnection.ExecuteAsync(insertQuery, parameters);

                return Json(new { success = true, message = "Comment added successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding comment: {ex.Message}");
                return Json(new { success = false, message = "Could not add comment." });
            }
        }

        public class TildelRapportModel
        {
            public int RapportId { get; set; }
            public int NyAnsattId { get; set; }
        }

    }
}