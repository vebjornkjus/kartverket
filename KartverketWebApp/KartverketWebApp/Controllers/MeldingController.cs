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
using System.Data.Common;

namespace KartverketWebApp.Controllers
{
    /// <summary>
    /// Controller for håndtering av meldinger mellom brukere i systemet
    /// Krever at brukeren er autentisert for å få tilgang til funksjonaliteten
    /// </summary>
    [Authorize]
    public class MeldingerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dbConnection;

        /// <summary>
        /// Konstruktør som initialiserer databasekontekst og tilkobling
        /// </summary>
        public MeldingerController(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Henter og viser alle meldinger for innlogget bruker
        /// Grupperer meldinger basert rapport og viser siste melding i hver samtale
        /// Returnerer forskjellige views basert på brukertype (saksbehandler eller vanlig bruker)
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Meldinger()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var bruker = await _context.Bruker
                .Include(b => b.Personer)
                .FirstOrDefaultAsync(b => b.Email == email);

            if (bruker == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var person = bruker.Personer.FirstOrDefault();
            if (person == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.UserName = person.Fornavn;
            ViewBag.UserLastName = person.Etternavn;
            ViewBag.UserEmail = email;

            var currentPersonId = person.PersonId;

            var messages = await _context.Meldinger
                .Where(m => (m.SenderPersonId == currentPersonId || m.MottakerPersonId == currentPersonId) &&
                       _context.Rapport.Any(r => r.RapportId == m.RapportId &&
                            (r.RapportStatus == "Uåpnet" || r.RapportStatus == "Under behandling")))
                .Include(m => m.Sender)
                .Include(m => m.Mottaker)
                .Include(m => m.Rapport.Kart)
                .OrderByDescending(m => m.Tidsstempel)
                .ToListAsync();

            var conversations = messages
                .GroupBy(m => m.RapportId)
                .Select(g =>
                {
                    var lastMessage = g.OrderByDescending(m => m.Tidsstempel).First(); // Henter nyeste melding i samtalen

                    return new
                    {
                        RapportId = g.Key,
                        Tittel = lastMessage.Rapport?.Kart?.Tittel ?? "Ukjent tittel",
                        LastMessage = lastMessage,
                        // Hvis siste melding er fra innlogget bruker, vis "Deg", ellers vis avsenderens navn
                        LastSenderName = lastMessage.SenderPersonId == currentPersonId
                            ? "Deg"
                            : $"{lastMessage.Sender.Fornavn} {lastMessage.Sender.Etternavn}",
                        Status = lastMessage.Status,
                        RecipientId = lastMessage.MottakerPersonId,
                        Tidsstempel = lastMessage.Tidsstempel
                    };
                })
                .OrderByDescending(c => c.Tidsstempel)
                .ToList();

            var combinedViewModel = new CombinedViewModel
            {
                SammtaleModel = conversations.Select(c => new SammtaleModel
                {
                    RapportId = c.RapportId,
                    Tittel = c.Tittel,
                    LastMessage = c.LastMessage.Innhold,
                    LastSenderName = c.LastSenderName,  // Dette vil nå være riktig basert på siste melding
                    Status = c.Status,
                    RecipientId = c.RecipientId,
                    Tidsstempel = c.Tidsstempel
                }).ToList()
            };

            return bruker.BrukerType == "saksbehandler"
                ? View("~/Views/Home/Saksbehandler/Meldinger.cshtml", combinedViewModel)
                : View("~/Views/Home/MeldingerMinSide.cshtml", combinedViewModel);
        }
        /// <summary>
        /// Henter alle meldinger i en spesifikk samtale/rapport
        /// </summary>
        /// <param name="rapportId">ID-en til rapporten man ønsker å se meldinger for</param>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetConversation(int rapportId)
        {
            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return BadRequest("User not authenticated properly.");
                }

                var bruker = await _context.Bruker
                    .Include(b => b.Personer)
                    .FirstOrDefaultAsync(b => b.Email == userEmail);

                if (bruker == null || !bruker.Personer.Any())
                {
                    return BadRequest("User profile not found.");
                }

                var personId = bruker.Personer.First().PersonId;
                var norwegianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

                var messages = await _context.Meldinger
                    .Where(m => m.RapportId == rapportId)
                    .Include(m => m.Sender)
                    .Include(m => m.Mottaker)
                    .OrderBy(m => m.Tidsstempel)
                    .Select(m => new
                    {
                        SenderName = m.Sender.Fornavn + " " + m.Sender.Etternavn,
                        Innhold = m.Innhold,
                        Tidsstempel = TimeZoneInfo.ConvertTimeFromUtc(
                            m.Tidsstempel.ToUniversalTime(),
                            norwegianTimeZone
                        ).ToString("dd.MM.yyyy HH:mm"),
                        IsSender = m.SenderPersonId == personId
                    })
                    .ToListAsync();

                return Json(messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetConversation: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest("Failed to load conversation.");
            }
        }

        /// <summary>
        /// Sender en ny melding i en samtale
        /// </summary>
        /// <param name="rapportId">ID-en til rapporten meldingen tilhører</param>
        /// <param name="mottakerPersonId">ID-en til personen som skal motta meldingen</param>
        /// <param name="innhold">Selve meldingsteksten</param>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] int rapportId, [FromForm] int mottakerPersonId, [FromForm] string innhold)
        {
            try
            {
                // Validerer at meldingen ikke er tom
                if (string.IsNullOrWhiteSpace(innhold))
                {
                    return Json(new { success = false, message = "Message content cannot be empty." });
                }

                // Validerer at rapportId er gyldig
                if (rapportId <= 0)
                {
                    return Json(new { success = false, message = "Invalid report ID." });
                }

                // Henter avsenderens informasjon
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Json(new { success = false, message = "User not authenticated properly." });
                }

                var bruker = await _context.Bruker
                    .Include(b => b.Personer)
                    .FirstOrDefaultAsync(b => b.Email == userEmail);

                if (bruker == null || !bruker.Personer.Any())
                {
                    return Json(new { success = false, message = "User profile not found." });
                }

                var senderPersonId = bruker.Personer.First().PersonId;

                // Verifiserer at rapport og mottaker eksisterer
                var rapportExists = await _context.Rapport.AnyAsync(r => r.RapportId == rapportId);
                var mottakerExists = await _context.Person.AnyAsync(p => p.PersonId == mottakerPersonId);

                if (!rapportExists || !mottakerExists)
                {
                    return Json(new { success = false, message = "Invalid report or recipient." });
                }

                var norwegianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, norwegianTimeZone);

                // Oppretter og lagrer ny melding
                var nyMelding = new Meldinger
                {
                    RapportId = rapportId,
                    SenderPersonId = senderPersonId,
                    MottakerPersonId = mottakerPersonId,
                    Innhold = innhold,
                    Tidsstempel = currentTime,
                    Status = "sendt"
                };

                _context.Meldinger.Add(nyMelding);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Message sent successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}
