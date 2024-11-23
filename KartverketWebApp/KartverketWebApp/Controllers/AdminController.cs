using Microsoft.AspNetCore.Mvc;
using KartverketWebApp.Models;
using Microsoft.EntityFrameworkCore;
using KartverketWebApp.Data;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace KartverketWebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbConnection _dbConnection;

        // Constructor for å bruke DbContext
        public AdminController(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
        }

        // Metode for å vise BrukerOversikt med filtrering
        public async Task<IActionResult> BrukerOversikt(string filter = null)
        {
            // Hvis filter er null, hent alle brukerne
            var brukerOversiktQuery = _context.Bruker
                .Join(_context.Person,
                    b => b.BrukerId,
                    p => p.BrukerId,
                    (b, p) => new BrukerOversiktViewModel
                    {
                        BrukerId = b.BrukerId,
                        Email = b.Email,
                        BrukerType = b.BrukerType,
                        Fornavn = p.Fornavn,
                        Etternavn = p.Etternavn
                    });

            // Hvis filter er satt, filtrer på BrukerType
            if (!string.IsNullOrEmpty(filter))
            {
                brukerOversiktQuery = brukerOversiktQuery.Where(b => b.BrukerType == filter);
            }

            var brukerOversikt = await brukerOversiktQuery.ToListAsync();

            return View("~/Views/Home/Admin/BrukerOversikt.cshtml", brukerOversikt);
        }

        // Metode for å slette en bruker
        [HttpPost]
        public IActionResult SlettBruker(int id)
        {
            var bruker = _context.Bruker.FirstOrDefault(b => b.BrukerId == id);
            var person = _context.Person.FirstOrDefault(p => p.BrukerId == id);

            if (bruker != null)
            {
                // Slett bruker og person
                _context.Bruker.Remove(bruker);
                if (person != null)
                {
                    _context.Person.Remove(person);
                }

                _context.SaveChanges();
            }

            // Etter sletting, send brukeren tilbake til BrukerOversikt-siden
            return RedirectToAction("BrukerOversikt");
        }


        // Metode for å vise redigeringsskjema
        [HttpGet]
        public IActionResult RedigerBruker(int id)
        {
            var bruker = _context.Bruker
                .Join(_context.Person,
                      b => b.BrukerId,
                      p => p.BrukerId,
                      (b, p) => new BrukerOversiktViewModel
                      {
                          BrukerId = b.BrukerId,
                          Email = b.Email,
                          BrukerType = b.BrukerType,
                          Fornavn = p.Fornavn,
                          Etternavn = p.Etternavn
                      })
                .FirstOrDefault(b => b.BrukerId == id);

            if (bruker == null)
            {
                return NotFound();
            }

            return View("~/Views/Home/Admin/RedigerBruker.cshtml", bruker);
        }

        // Metode for å oppdatere brukerdata
        [HttpPost]
        public IActionResult RedigerBruker(BrukerOversiktViewModel model)
        {
            var bruker = _context.Bruker.FirstOrDefault(b => b.BrukerId == model.BrukerId);
            var person = _context.Person.FirstOrDefault(p => p.BrukerId == model.BrukerId);

            if (bruker != null && person != null)
            {
                // Oppdater brukerdata
                bruker.Email = model.Email;
                bruker.BrukerType = model.BrukerType;
                person.Fornavn = model.Fornavn;
                person.Etternavn = model.Etternavn;

                _context.SaveChanges();
                return RedirectToAction("BrukerOversikt");
            }

            return View("~/Views/Home/Admin/RedigerBruker.cshtml", model);
        }
    }
}