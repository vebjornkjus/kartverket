using Microsoft.AspNetCore.Mvc;
using KartverketWebApp.Models;
using Microsoft.EntityFrameworkCore;
using KartverketWebApp.Data;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

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
                    (b, p) => new AdminViewModel
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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var bruker = await _context.Bruker
                    .Include(b => b.Personer)
                    .FirstOrDefaultAsync(b => b.BrukerId == id);

                if (bruker != null)
                {
                    // Get the deleted user or create if not exists
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
                            Passord = "SlettetBruker123!",
                            Personer = new List<Person> { slettetPerson }
                        };

                        _context.Bruker.Add(slettetBruker);
                        await _context.SaveChangesAsync();
                    }

                    // For each person associated with the user being deleted
                    foreach (var person in bruker.Personer)
                    {
                        // Handle Rapporter
                        var rapporter = await _context.Rapport
                            .Where(r => r.PersonId == person.PersonId)
                            .ToListAsync();
                        foreach (var rapport in rapporter)
                        {
                            rapport.PersonId = slettetBruker.Personer.First().PersonId;
                        }

                        // Handle Meldinger where person is sender
                        var sendteMeldinger = await _context.Meldinger
                            .Where(m => m.SenderPersonId == person.PersonId)
                            .ToListAsync();
                        foreach (var melding in sendteMeldinger)
                        {
                            melding.SenderPersonId = slettetBruker.Personer.First().PersonId;
                        }

                        // Handle Meldinger where person is receiver
                        var mottatteMeldinger = await _context.Meldinger
                            .Where(m => m.MottakerPersonId == person.PersonId)
                            .ToListAsync();
                        foreach (var melding in mottatteMeldinger)
                        {
                            melding.MottakerPersonId = slettetBruker.Personer.First().PersonId;
                        }

                        // Handle Ansatt records
                        var ansatte = await _context.Ansatt
                            .Where(a => a.PersonId == person.PersonId)
                            .ToListAsync();
                        foreach (var ansatt in ansatte)
                        {
                            // Handle Rapporter assigned to this employee
                            var tildeltRapporter = await _context.Rapport
                                .Where(r => r.TildelAnsattId == ansatt.AnsattId)
                                .ToListAsync();
                            foreach (var rapport in tildeltRapporter)
                            {
                                rapport.TildelAnsattId = null;  // Or assign to another employee if needed
                            }

                            _context.Ansatt.Remove(ansatt);
                        }

                        // Remove the person
                        _context.Person.Remove(person);
                    }

                    // Finally remove the user
                    _context.Bruker.Remove(bruker);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction("BrukerOversikt");
                }

                await transaction.RollbackAsync();
                return RedirectToAction("BrukerOversikt", new { error = "Bruker ikke funnet." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the error
                return RedirectToAction("BrukerOversikt", new { error = "Kunne ikke slette bruker." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> RedigerBruker(int id)
        {
            var bruker = await _context.Bruker
                .Join(_context.Person,
                      b => b.BrukerId,
                      p => p.BrukerId,
                      (b, p) => new AdminViewModel
                      {
                          BrukerId = b.BrukerId,
                          Email = b.Email,
                          BrukerType = b.BrukerType,
                          Fornavn = p.Fornavn,
                          Etternavn = p.Etternavn,

                          Kommunenummer = _context.Ansatt
                      .Where(a => a.PersonId == p.PersonId)
                      .Select(a => a.Kommunenummer)
                      .FirstOrDefault()
                      })
                .FirstOrDefaultAsync(b => b.BrukerId == id);

            if (bruker == null)
            {
                return NotFound();
            }

            return View("~/Views/Home/Admin/RedigerBruker.cshtml", bruker);
        }

        [HttpPost]
        public async Task<IActionResult> RedigerBruker(AdminViewModel model)
        {
            var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.BrukerId == model.BrukerId);
            var person = await _context.Person.FirstOrDefaultAsync(p => p.BrukerId == model.BrukerId);

            if (bruker != null && person != null)
            {
                // Oppdater brukerdata
                bruker.Email = model.Email;
                bruker.BrukerType = model.BrukerType;
                person.Fornavn = model.Fornavn;
                person.Etternavn = model.Etternavn;

                // Hvis brukertype er "saksbehandler"
                if (model.BrukerType == "saksbehandler" && model.Kommunenummer.HasValue)
                {
                    var eksisterendeAnsatt = await _context.Ansatt.FirstOrDefaultAsync(a => a.PersonId == person.PersonId);

                    if (eksisterendeAnsatt == null)
                    {
                        // Opprett ny ansattoppføring
                        var ansatt = new Ansatt
                        {
                            PersonId = person.PersonId,
                            Kommunenummer = model.Kommunenummer.Value,
                            AnsettelsesDato = DateTime.Now
                        };
                        _context.Ansatt.Add(ansatt);
                    }
                    else
                    {
                        // Oppdater eksisterende ansattoppføring
                        eksisterendeAnsatt.Kommunenummer = model.Kommunenummer.Value;
                        _context.Ansatt.Update(eksisterendeAnsatt);
                    }
                }
                else
                {
                    // Fjern eventuell ansattoppføring hvis brukertype ikke er "saksbehandler"
                    var ansatt = await _context.Ansatt.FirstOrDefaultAsync(a => a.PersonId == person.PersonId);
                    if (ansatt != null)
                    {
                        _context.Ansatt.Remove(ansatt);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("BrukerOversikt");
            }

            return View("~/Views/Home/Admin/RedigerBruker.cshtml", model);
        }
    }
}