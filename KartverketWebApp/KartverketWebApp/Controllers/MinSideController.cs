using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using KartverketWebApp.Data;
using KartverketWebApp.Models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace KartverketWebApp.Controllers
{
    [Authorize]
    public class MinSideController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<MinSideController> _logger;

        public MinSideController(ApplicationDbContext context, IDbConnection dbConnection, ILogger<MinSideController> logger)
        {
            _context = context;
            _dbConnection = dbConnection;
            _logger = logger;
        }

    [HttpGet]
    public IActionResult Index()
    {
        var userEmail = User.Identity?.Name;

        var brukerInfo = _context.Bruker
            .Join(_context.Person,
                b => b.BrukerId,
                p => p.BrukerId,
                (b, p) => new MinSideViewModel
                {
                    BrukerId = b.BrukerId,
                    Email = b.Email,
                    BrukerType = b.BrukerType,
                    Fornavn = p.Fornavn,
                    Etternavn = p.Etternavn
                })
            .FirstOrDefault(b => b.Email == userEmail);

        if (brukerInfo == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Hent rapporter
        var rapporter = _context.Rapport
            .Include(r => r.Kart)
            .Where(r => r.PersonId == _context.Person
                .Where(p => p.BrukerId == brukerInfo.BrukerId)
                .Select(p => p.PersonId)
                .FirstOrDefault())
            .ToList();

        brukerInfo.Rapporter = rapporter;

        return View("~/Views/Home/MinSide.cshtml", brukerInfo);
    }

    [HttpPost]
[HttpPost]
public async Task<IActionResult> SlettBruker(int id)
{
    var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.BrukerId == id);
    var person = await _context.Person.FirstOrDefaultAsync(p => p.BrukerId == id);

    if (bruker != null)
    {
        // Finn eller opprett "Slettet Bruker"
        var slettetBruker = await _context.Bruker
            .Include(b => b.Personer)
            .FirstOrDefaultAsync(b => b.Email == "slettet.bruker@kartverket.no");

        if (slettetBruker == null)
        {
            var slettetPerson = new Person
            {
                Fornavn = "Slettet",
                Etternavn = "Bruker"
            };

            slettetBruker = new Bruker
            {
                Email = "slettet.bruker@kartverket.no",
                BrukerType = "Slettet",
                Passord = "SlettetBruker123!", // Lagt til passord
                Personer = new List<Person> { slettetPerson }
            };

            _context.Bruker.Add(slettetBruker);
            await _context.SaveChangesAsync();
        }

        // Finn rapporter knyttet til personen som skal slettes
        if (person != null)
        {
            var rapporter = await _context.Rapport
                .Where(r => r.PersonId == person.PersonId)
                .ToListAsync();

            // Overfør rapportene til slettet bruker
            foreach (var rapport in rapporter)
            {
                rapport.PersonId = slettetBruker.Personer.First().PersonId;
                _context.Update(rapport);
            }
        }

        // Slett bruker og person
        if (person != null)
        {
            _context.Person.Remove(person);
        }
        _context.Bruker.Remove(bruker);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Logg feilen hvis lagring mislykkes
            _logger.LogError($"Feil oppstod under sletting av bruker og person: {ex.Message}");
            return StatusCode(500, "En feil oppstod under slettingen.");
        }

        // Logg ut brukeren
        await HttpContext.SignOutAsync("AuthCookie");


        // Nullstill brukerens identitet
        HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

        // Rediriger til startsiden
        return RedirectToAction("Index", "Home");
    }

    // Hvis bruker ikke finnes, rediriger til startsiden
    return RedirectToAction("Index", "Home");
}}}
