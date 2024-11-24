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
    /// <summary>
    /// Kontroller for håndtering av rapportstatus og tilknyttede operasjoner
    /// Krever at brukeren er enten admin eller saksbehandler
    /// </summary>
    [Authorize(Policy = "AdminOrSaksbehandlerPolicy")]
    public class DetaljertController : Controller
    {
        // Privat felt for avhengigheter som brukes i kontrolleren
        private readonly ILogger<DetaljertController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dbConnection;
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;

        /// <summary>
        /// Konstruktør som initialiserer alle nødvendige tjenester og avhengigheter
        /// </summary>
        public DetaljertController(
            ILogger<DetaljertController> logger,
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

        /// <summary>
        /// Setter status på en rapport til 'Avklart' og oppdaterer behandlingsdato
        /// </summary>
        [Authorize(Policy = "AdminOrSaksbehandlerPolicy")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetFjernet(int rapportId)
        {
            try
            {
                // SQL-spørring for å oppdatere rapportens status til 'Fjernet' og sette behandlingsdato
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

                await _dbConnection.ExecuteAsync(query, parameters);
                _logger.LogInformation($"Rapport {rapportId} er satt til Fjernet");

                return RedirectToAction("Saksbehandler", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Feil ved setting av rapport {rapportId} til Fjernet: {ex.Message}");
                TempData["Error"] = "Det oppstod en feil ved oppdatering av rapporten.";
                return RedirectToAction("Saksbehandler", "Home");
            }
        }

        [Authorize(Policy = "AdminOrSaksbehandlerPolicy")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetAvklart(int rapportId)
        {
            try
            {
                // SQL-spørring for å oppdatere rapportens status til 'Avklart' og sette behandlingsdato
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

                await _dbConnection.ExecuteAsync(query, parameters);
                _logger.LogInformation($"Rapport {rapportId} er satt til Avklart");

                return RedirectToAction("Saksbehandler", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Feil ved setting av rapport {rapportId} til Avklart: {ex.Message}");
                TempData["Error"] = "Det oppstod en feil ved oppdatering av rapporten.";
                return RedirectToAction("Saksbehandler", "Home");
            }
        }

        /// <summary>
        /// Henter liste over tilgjengelige saksbehandlere i samme kommune som innlogget bruker
        /// Returnerer ikke den innloggede brukeren i listen
        /// Brukes for å hente inn mulige ansatte som kan over ta rapporten
        /// </summary>
        [Authorize]
        [HttpGet]
        public IActionResult HentTilgjengeligeAnsatte()
        {
            try
            {
                // Henter kommunenummer for innlogget bruker
                const string userQuery = @"
            SELECT DISTINCT a.Kommunenummer
            FROM Ansatt a
            INNER JOIN Person p ON a.PersonId = p.PersonId
            INNER JOIN Bruker b ON p.BrukerId = b.BrukerId
            WHERE b.Email = @UserEmail";

                var userEmail = User.Identity.Name;
                var userKommunenummer = _dbConnection.QueryFirstOrDefault<int>(userQuery, new { UserEmail = userEmail });

                // Henter alle saksbehandlere fra samme kommune, unntatt innlogget bruker
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
            AND b.Email != @CurrentUserEmail
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

        /// <summary>
        /// Sender en rapport til en ny saksbehandler
        /// Oppdaterer også rapportstatus til 'Under behandling' hvis den var 'Uåpnet'
        /// </summary>
        /// <param name="model">Inneholder RapportId og NyAnsattId for tildelingen</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TildelRapport([FromBody] TildelRapportModel model)
        {
            try
            {
                // SQL-spørring for å oppdatere rapportstatus og tildele den til en ny saksbehandler
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

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Saksbehandler", "Home")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Feil ved tildeling av rapport {model.RapportId} til ansatt {model.NyAnsattId}: {ex.Message}");
                return Json(new { success = false, error = "Kunne ikke tildele rapport." });
            }
        }

        /// <summary>
        /// Oppretter en ny meldeig bassert på RapportId
        /// Oppretter en melding mellom avsender og mottaker i databasen
        /// </summary>
        /// <param name="innhold">Innholdet i kommentaren</param>
        /// <param name="rapportId">ID-en til rapporten kommentaren tilhører</param>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromForm] string innhold, [FromForm] int rapportId)
        {
            try
            {
                // Validerer at kommentaren ikke er tom
                if (string.IsNullOrWhiteSpace(innhold))
                {
                    return Json(new { success = false, message = "Comment content cannot be empty." });
                }

                // Henter innlogget brukers e-post
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Json(new { success = false, message = "User not authenticated properly." });
                }

                // Henter avsenders PersonId
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

                // Henter mottakers PersonId
                const string recipientQuery = @"
            SELECT PersonId
            FROM Rapport
            WHERE RapportId = @RapportId";

                var mottakerPersonId = await _dbConnection.QueryFirstOrDefaultAsync<int?>(recipientQuery, new { RapportId = rapportId });

                if (!mottakerPersonId.HasValue)
                {
                    return Json(new { success = false, message = "Rapport not found." });
                }

                // Lagrer ny melding i databasen
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

        /// <summary>
        /// Modell for å tildele en rapport til en ny saksbehandler
        /// </summary>
        public class TildelRapportModel
        {
            public int RapportId { get; set; }
            public int NyAnsattId { get; set; }
        }
    }
}