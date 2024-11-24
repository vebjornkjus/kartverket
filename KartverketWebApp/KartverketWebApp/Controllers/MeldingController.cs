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
    [Authorize]
    public class MeldingerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dbConnection;

        public MeldingerController(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
        }

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

            // Fetch all relevant messages
            var messages = await _context.Meldinger
                .Where(m => _context.Rapport.Any(r => r.RapportId == m.RapportId && (r.RapportStatus == "Uåpnet" || r.RapportStatus == "Under behandling")))
                .Where(m => m.SenderPersonId == currentPersonId || m.MottakerPersonId == currentPersonId)
                .Include(m => m.Sender)
                .Include(m => m.Mottaker)
                .Include(m => m.Rapport.Kart)
                .OrderByDescending(m => m.Tidsstempel)
                .ToListAsync();

            // Group messages by rapport ID
            var conversations = messages
                .GroupBy(m => m.RapportId)
                .Select(g => new
                {
                    RapportId = g.Key,
                    Tittel = g.FirstOrDefault()?.Rapport?.Kart?.Tittel ?? "Ukjent tittel",
                    LastMessage = g.FirstOrDefault(),
                    SenderName = g.FirstOrDefault().SenderPersonId == currentPersonId
                        ? "Deg"
                        : g.FirstOrDefault().MottakerPersonId == currentPersonId
                            ? g.FirstOrDefault().Sender.Fornavn + " " + g.FirstOrDefault().Sender.Etternavn
                            : g.FirstOrDefault().Mottaker.Fornavn + " " + g.FirstOrDefault().Mottaker.Etternavn,
                    LastSenderName = g.OrderByDescending(m => m.Tidsstempel).FirstOrDefault().SenderPersonId == currentPersonId
                        ? "Deg"
                        : g.OrderByDescending(m => m.Tidsstempel).FirstOrDefault().Sender.Fornavn + " " + g.OrderByDescending(m => m.Tidsstempel).FirstOrDefault().Sender.Etternavn,
                    Status = g.OrderByDescending(m => m.Tidsstempel).FirstOrDefault().Status,
                    RecipientId = g.FirstOrDefault().MottakerPersonId
                })
                .OrderByDescending(c => c.LastMessage.Tidsstempel)
                .ToList();

            var combinedViewModel = new CombinedViewModel
            {
                SammtaleModel = conversations.Select(c => new SammtaleModel
                {
                    RapportId = c.RapportId,
                    Tittel = c.Tittel,
                    LastMessage = c.LastMessage?.Innhold ?? "Ingen melding",
                    SenderName = c.SenderName,
                    LastSenderName = c.LastSenderName,
                    Status = c.Status,
                    RecipientId = c.RecipientId
                }).ToList()
            };

            return View("~/Views/Home/Saksbehandler/Meldinger.cshtml", combinedViewModel);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetConversation(int rapportId)
        {
            try
            {
                // Get the user's email from claims
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return BadRequest("User not authenticated properly.");
                }

                // Get the user's PersonId from Bruker and Person tables
                var bruker = await _context.Bruker
                    .Include(b => b.Personer)
                    .FirstOrDefaultAsync(b => b.Email == userEmail);

                if (bruker == null || !bruker.Personer.Any())
                {
                    return BadRequest("User profile not found.");
                }

                var personId = bruker.Personer.First().PersonId;

                var messages = await _context.Meldinger
                    .Where(m => m.RapportId == rapportId)
                    .Include(m => m.Sender)
                    .Include(m => m.Mottaker)
                    .OrderBy(m => m.Tidsstempel)
                    .Select(m => new
                    {
                        SenderName = m.Sender.Fornavn + " " + m.Sender.Etternavn,
                        Innhold = m.Innhold,
                        Tidsstempel = m.Tidsstempel.ToString("dd.MM.yyyy HH:mm"),
                        IsSender = m.SenderPersonId == personId  // Using personId instead of userId
                    })
                    .ToListAsync();

                // Add debug logging
                foreach (var message in messages)
                {
                    Console.WriteLine($"Message: {message.Innhold}, IsSender: {message.IsSender}, SenderPersonId: {message.SenderName}");
                }

                return Json(messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetConversation: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return BadRequest("Failed to load conversation.");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] int rapportId, [FromForm] int mottakerPersonId, [FromForm] string innhold)
        {
            Console.WriteLine($"SendMessage called with: RapportId={rapportId}, MottakerPersonId={mottakerPersonId}, Innhold={innhold}");

            try
            {
                if (string.IsNullOrWhiteSpace(innhold))
                {
                    return Json(new { success = false, message = "Message content cannot be empty." });
                }

                // Get the user's email from claims
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Json(new { success = false, message = "User not authenticated properly." });
                }

                // Get the user's PersonId from Bruker and Person tables
                var bruker = await _context.Bruker
                    .Include(b => b.Personer)
                    .FirstOrDefaultAsync(b => b.Email == userEmail);

                if (bruker == null || !bruker.Personer.Any())
                {
                    return Json(new { success = false, message = "User profile not found." });
                }

                var senderPersonId = bruker.Personer.First().PersonId;
                Console.WriteLine($"Found sender PersonId: {senderPersonId}");

                // Verify the related entities exist
                var rapportExists = await _context.Rapport.AnyAsync(r => r.RapportId == rapportId);
                var mottakerExists = await _context.Person.AnyAsync(p => p.PersonId == mottakerPersonId);

                if (!rapportExists)
                {
                    return Json(new { success = false, message = $"Rapport with ID {rapportId} does not exist." });
                }

                if (!mottakerExists)
                {
                    return Json(new { success = false, message = $"Mottaker with ID {mottakerPersonId} does not exist." });
                }

                var nyMelding = new Meldinger
                {
                    RapportId = rapportId,
                    SenderPersonId = senderPersonId,
                    MottakerPersonId = mottakerPersonId,
                    Innhold = innhold,
                    Tidsstempel = DateTime.Now,
                    Status = "sendt"
                };

                _context.Meldinger.Add(nyMelding);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Message sent successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }



        [HttpPost]
        public IActionResult UpdateMessageStatus(int id)
        {
            // SQL query to update the status
            string query = @"
        UPDATE Meldinger
        SET Status = 'åpnet'
        WHERE MeldingsId = @MeldingsId AND Status = 'sendt';
    ";

            // Execute the query
            var rowsAffected = _dbConnection.Execute(query, new { MeldingsId = id });

            // Return a simple success or failure response
            return Json(new { success = rowsAffected > 0 });
        }
    }
}
