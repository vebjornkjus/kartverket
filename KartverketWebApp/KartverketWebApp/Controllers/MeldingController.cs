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
namespace KartverketWebApp.Controllers
{
    [Authorize]
    public class MeldingerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeldingerController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize]
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

            // Fetch conversations
            var conversations = await _context.Meldinger
                .Where(m => _context.Rapport
                    .Where(r => r.RapportStatus == "Uåpnet" || r.RapportStatus == "Under behandling")
                    .Any(r => r.RapportId == m.RapportId))
                .GroupBy(m => m.RapportId)
                .Select(group => new
                {
                    RapportId = group.Key,
                    Tittel = _context.Rapport
                        .Where(r => r.RapportId == group.Key)
                        .Select(r => r.Kart.Tittel)
                        .FirstOrDefault(),
                    LastMessage = group.OrderByDescending(m => m.Tidsstempel).FirstOrDefault(),
                    SenderName = group.FirstOrDefault().Mottaker.Fornavn + " " + group.FirstOrDefault().Mottaker.Etternavn, // Name of the person you're having a conversation with
                    LastSenderName = group.OrderByDescending(m => m.Tidsstempel)
                        .Select(m => m.Sender.Fornavn + " " + m.Sender.Etternavn)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // Map to SammtaleModel
            var currentPersonId = person.PersonId; // ID of the logged-in person
            var combinedViewModel = new CombinedViewModel
            {
                SammtaleModel = conversations.Select(c => new SammtaleModel
                {
                    RapportId = c.RapportId,
                    Tittel = c.Tittel,
                    LastMessage = c.LastMessage?.Innhold,
                    SenderName = c.SenderName,
                    LastSenderName = c.LastMessage?.SenderPersonId == currentPersonId ? "Deg" : c.LastSenderName
                }).ToList()
            };

            return View("~/Views/Home/Saksbehandler/Meldinger.cshtml", combinedViewModel);

        }






            // Send a new message
            [HttpPost]
        public async Task<IActionResult> SendMessage(int rapportId, int mottakerPersonId, string innhold)
        {
            var senderPersonId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (senderPersonId == 0 || string.IsNullOrWhiteSpace(innhold))
            {
                return BadRequest("Invalid data");
            }

            var nyMelding = new Meldinger
            {
                RapportId = rapportId,
                SenderPersonId = senderPersonId,
                MottakerPersonId = mottakerPersonId,
                Innhold = innhold,
                Tidsstempel = DateTime.Now,
                Status = "Sendt"
            };

            _context.Meldinger.Add(nyMelding);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { rapportId });
        }
    }

}
