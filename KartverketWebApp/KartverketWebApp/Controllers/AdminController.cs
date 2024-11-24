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

        public AdminController(ApplicationDbContext context, IDbConnection dbConnection)
        {
            _context = context;
            _dbConnection = dbConnection;
        }

        public async Task<IActionResult> BrukerOversikt(string filter = null)
        {
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

            if (!string.IsNullOrEmpty(filter))
            {
                brukerOversiktQuery = brukerOversiktQuery.Where(b => b.BrukerType == filter);
            }

            var brukerOversikt = await brukerOversiktQuery.ToListAsync();

            return View("~/Views/Home/Admin/BrukerOversikt.cshtml", brukerOversikt);
        }

        [HttpPost]
        public async Task<IActionResult> SlettBruker(int id)
        {
            var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.BrukerId == id);
            var person = await _context.Person.FirstOrDefaultAsync(p => p.BrukerId == id);

            if (bruker != null)
            {
                _context.Bruker.Remove(bruker);
                if (person != null)
                {
                    _context.Person.Remove(person);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("BrukerOversikt");
        }

        [HttpGet]
        public async Task<IActionResult> RedigerBruker(int id)
        {
            var bruker = await _context.Bruker
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
                .FirstOrDefaultAsync(b => b.BrukerId == id);

            if (bruker == null)
            {
                return NotFound();
            }

            return View("~/Views/Home/Admin/RedigerBruker.cshtml", bruker);
        }

        [HttpPost]
        public async Task<IActionResult> RedigerBruker(BrukerOversiktViewModel model)
        {
            var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.BrukerId == model.BrukerId);
            var person = await _context.Person.FirstOrDefaultAsync(p => p.BrukerId == model.BrukerId);

            if (bruker != null && person != null)
            {
                bruker.Email = model.Email;
                bruker.BrukerType = model.BrukerType;
                person.Fornavn = model.Fornavn;
                person.Etternavn = model.Etternavn;

                await _context.SaveChangesAsync();
                return RedirectToAction("BrukerOversikt");
            }

            return View("~/Views/Home/Admin/RedigerBruker.cshtml", model);
        }
    }
}