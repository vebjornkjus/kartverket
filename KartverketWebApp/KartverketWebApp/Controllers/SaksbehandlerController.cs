using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using KartverketWebApp.Models;
using KartverketWebApp.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace KartverketWebApp.Controllers
{

    public class RapportViewModel
    {
        public int RapportId { get; set; }
        public string RapportStatus { get; set; }
        public DateTime Opprettet { get; set; }
        public string Tittel { get; set; }
        public string MapType { get; set; }
    }

    public class SaksbehandlerController : Controller
    {
        private readonly ILogger<SaksbehandlerController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;

        public SaksbehandlerController(
            ILogger<SaksbehandlerController> logger,
            ApplicationDbContext context,
            IConfiguration configuration,
            IDbConnection dbConnection)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _dbConnection = dbConnection;
        }


        [Authorize(Policy = "AdminOrSaksbehandlerPolicy")]
        [HttpGet]
        public async Task<IActionResult> TidligereRapporter(int avklartPage = 1, int fjernetPage = 1, int pageSize = 10)
        {
            using var connection = _dbConnection;
            connection.Open();

            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("User email not found in claims.");
                return Unauthorized("User not logged in.");
            }

            _logger.LogInformation($"Fetching data for user email: {userEmail}");

            const string userQuery = @"
        SELECT b.*, p.*, a.*
        FROM Bruker b
        JOIN Person p ON p.BrukerId = b.BrukerId
        JOIN Ansatt a ON a.PersonId = p.PersonId
        WHERE b.Email = @Email";

            var userInfo = await connection.QueryFirstOrDefaultAsync<dynamic>(userQuery, new { Email = userEmail });

            if (userInfo == null)
            {
                _logger.LogWarning($"No user found for email: {userEmail}");
                return NotFound("User not found.");
            }

            ViewBag.UserName = userInfo.Fornavn;
            ViewBag.UserLastName = userInfo.Etternavn;
            ViewBag.UserEmail = userEmail;

            var avklartOffset = (avklartPage - 1) * pageSize;
            var fjernetOffset = (fjernetPage - 1) * pageSize;

            const string avklartQuery = @"
        SELECT 
            r.RapportId, 
            r.RapportStatus, 
            r.Opprettet, 
            k.Tittel,
            k.MapType
        FROM Rapport r
        LEFT JOIN Kart k ON k.KartEndringId = r.KartEndringId
        WHERE r.TildelAnsattId = @AnsattId 
        AND r.RapportStatus = 'Avklart'
        ORDER BY r.Opprettet DESC
        LIMIT @PageSize OFFSET @Offset";

            var avklartParams = new
            {
                AnsattId = userInfo.AnsattId,
                Offset = avklartOffset,
                PageSize = pageSize
            };

            var avklartReports = (await connection.QueryAsync<RapportViewModel>(avklartQuery, avklartParams)).ToList();

            const string fjernetQuery = @"
        SELECT 
            r.RapportId, 
            r.RapportStatus, 
            r.Opprettet, 
            k.Tittel,
            k.MapType
        FROM Rapport r
        LEFT JOIN Kart k ON k.KartEndringId = r.KartEndringId
        WHERE r.TildelAnsattId = @AnsattId 
        AND r.RapportStatus = 'Fjernet'
        ORDER BY r.Opprettet DESC
        LIMIT @PageSize OFFSET @Offset";

            var fjernetParams = new
            {
                AnsattId = userInfo.AnsattId,
                Offset = fjernetOffset,
                PageSize = pageSize
            };

            var fjernetReports = (await connection.QueryAsync<RapportViewModel>(fjernetQuery, fjernetParams)).ToList();

            const string avklartCountQuery = @"
        SELECT COUNT(*) as count
        FROM Rapport
        WHERE TildelAnsattId = @AnsattId
        AND RapportStatus = 'Avklart'";

            var avklartCount = await connection.QueryFirstAsync<int>(avklartCountQuery, new { AnsattId = userInfo.AnsattId });

            const string fjernetCountQuery = @"
        SELECT COUNT(*) as count
        FROM Rapport
        WHERE TildelAnsattId = @AnsattId
        AND RapportStatus = 'Fjernet'";

            var fjernetCount = await connection.QueryFirstAsync<int>(fjernetCountQuery, new { AnsattId = userInfo.AnsattId });

            var viewModel = new CombinedViewModel
            {
                AvklartRapporter = avklartReports,
                FjernetRapporter = fjernetReports
            };

            ViewBag.AvklartCurrentPage = avklartPage;
            ViewBag.AvklartTotalPages = (int)Math.Ceiling((double)avklartCount / pageSize);

            ViewBag.FjernetCurrentPage = fjernetPage;
            ViewBag.FjernetTotalPages = (int)Math.Ceiling((double)fjernetCount / pageSize);

            return View("~/Views/Home/Saksbehandler/TidligereRapporter.cshtml", viewModel);
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

            // Find the user in the database using Dapper
            var brukerQuery = "SELECT * FROM Bruker WHERE Email = @Email";
            var bruker = await _dbConnection.QueryFirstOrDefaultAsync<Bruker>(brukerQuery, new { Email = userEmail });
            if (bruker == null)
            {
                _logger.LogWarning($"No Bruker found for email: {userEmail}");
                return NotFound("User not found.");
            }

            // Fetch the associated Person and Ansatt entities using Dapper
            var personQuery = "SELECT * FROM Person WHERE BrukerId = @BrukerId";
            var person = await _dbConnection.QueryFirstOrDefaultAsync<Person>(personQuery, new { BrukerId = bruker.BrukerId });
            if (person == null)
            {
                _logger.LogWarning($"No Person found for BrukerId: {bruker.BrukerId}");
                return NotFound("Person not found.");
            }

            var ansattQuery = "SELECT * FROM Ansatt WHERE PersonId = @PersonId";
            var ansatt = await _dbConnection.QueryFirstOrDefaultAsync<Ansatt>(ansattQuery, new { PersonId = person.PersonId });
            if (ansatt == null || ansatt.Kommunenummer == 0)
            {
                _logger.LogWarning($"Ansatt not found or invalid kommunenummer for PersonId: {person.PersonId}");
                return NotFound("Ansatt or kommunenummer not found.");
            }

            // Add user information to ViewBag
            ViewBag.UserName = person.Fornavn;
            ViewBag.UserLastName = person.Etternavn;
            ViewBag.UserEmail = userEmail;

            // Fetch only active reports using Dapper
            var activeReportsQuery = @"
        SELECT r.*, k.*
        FROM Rapport r
        LEFT JOIN Kart k ON r.KartEndringId = k.KartEndringId
        WHERE r.TildelAnsattId = @AnsattId
        AND (r.RapportStatus = 'Uåpnet' OR r.RapportStatus = 'Under behandling')
        ORDER BY r.Opprettet DESC
        LIMIT @PageSize OFFSET @Offset
    ";

            var activeReports = (await _dbConnection.QueryAsync<Rapport, Kart, Rapport>(activeReportsQuery,
                (rapport, kart) =>
                {
                    rapport.Kart = kart;
                    return rapport;
                },
                new { AnsattId = ansatt.AnsattId, Offset = (activePage - 1) * pageSize, PageSize = pageSize },
                splitOn: "KartEndringId")).ToList();

            // Fetch the total count of active reports using Dapper
            var totalActiveReportsQuery = "SELECT COUNT(*) FROM Rapport WHERE TildelAnsattId = @AnsattId AND (RapportStatus = 'Uåpnet' OR RapportStatus = 'Under behandling')";
            var totalActiveReports = await _dbConnection.QueryFirstAsync<int>(totalActiveReportsQuery, new { AnsattId = ansatt.AnsattId });

            // Populate the CombinedViewModel
            var combinedViewModel = new CombinedViewModel
            {
                ActiveRapporter = activeReports
            };

            ViewBag.ActiveCurrentPage = activePage;
            ViewBag.ActiveTotalPages = (int)Math.Ceiling((double)totalActiveReports / pageSize);

            return View("~/Views/Home/Saksbehandler/MineRapporter.cshtml", combinedViewModel);
        }
    }
}
