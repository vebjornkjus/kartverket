using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using KartverketWebApp.Data; // Inkluder namespace for ApplicationDbContext
using KartverketWebApp.Models; // Inkluder namespace for Bruker-modellen
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace KartverketWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<IdentityUser> _passwordHasher;

        public AccountController(
            ApplicationDbContext context,
            IPasswordHasher<IdentityUser> passwordHasher) // Initialiser passwordHasher
        {
            _context = context;
            _passwordHasher = passwordHasher; // Initialiser passwordHasher
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ekstra validering for e-postformat
            if (!ErEpostGyldig(model.Username))
            {
                ModelState.AddModelError(string.Empty, "Ugyldig e-postformat. Sørg for at e-postadressen er riktig skrevet.");
                return View(model);
            }

            // Sjekk e-post i Bruker-tabellen
            var bruker = _context.Bruker.FirstOrDefault(b => b.Email == model.Username);
            if (bruker == null)
            {
                ModelState.AddModelError(string.Empty, "Feil e-postadresse eller passord.");
                return View(model);
            }

            // Verifiser passord
            var isPasswordValid = false;
            if (bruker.Passord == model.Password) // Sjekk klartekst (kun for testing)
            {
                isPasswordValid = true;
            }
            else
            {
                var hashedValidationResult = _passwordHasher.VerifyHashedPassword(null, bruker.Passord, model.Password);
                if (hashedValidationResult == PasswordVerificationResult.Success)
                {
                    isPasswordValid = true;
                }
            }

            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Feil e-postadresse eller passord.");
                return View(model);
            }

            // Hent fornavn fra Person-tabellen
            var person = _context.Person.FirstOrDefault(p => p.BrukerId == bruker.BrukerId);

            // Legg til e-post og fornavn i claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, bruker.Email), // Bruk e-post som identifikator
                new Claim("Fornavn", person?.Fornavn ?? "Ukjent"), // Legg til fornavnet
                new Claim("BrukerType", bruker.BrukerType) // Valgfritt: Brukerrollen
            };

            var claimsIdentity = new ClaimsIdentity(claims, "AuthCookie");

            // Sett opp autentisering med claims
            await HttpContext.SignInAsync("AuthCookie", new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTime.UtcNow.AddHours(1)
            });

            return RedirectToAction("Index", "Home");
        }

        // Ny metode for å validere e-post
        private bool ErEpostGyldig(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            // Regex for å validere e-postformat
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return regex.IsMatch(email);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Legg til passordkrav
            if (!ErPassordGyldig(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Passordet må være minst 8 tegn langt, inneholde minst én stor bokstav, én liten bokstav,og ett tall");
                return View(model);
            }

            // Hash passordet
            var hashedPassword = _passwordHasher.HashPassword(null, model.Password);

            // Opprett Bruker-objektet
            var bruker = new Bruker
            {
                Email = model.Email,
                Passord = hashedPassword, // Lagre hashet passord
                BrukerType = "Standard" // Standard brukerrolle
            };

            // Legg til Bruker i databasen
            _context.Bruker.Add(bruker);
            await _context.SaveChangesAsync();

            // Opprett Person-objektet
            var person = new Person
            {
                Fornavn = model.Fornavn,
                Etternavn = model.Etternavn,
                BrukerId = bruker.BrukerId // Referanse til Bruker
            };

            // Legg til Person i databasen
            _context.Person.Add(person);
            await _context.SaveChangesAsync();

            // Logg inn brukeren automatisk etter registrering
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, bruker.Email),
                new Claim("Fornavn", model.Fornavn),
                new Claim("BrukerType", bruker.BrukerType)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "AuthCookie");

            await HttpContext.SignInAsync("AuthCookie", new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            {
                IsPersistent = false, // Midlertidig innlogging
                ExpiresUtc = DateTime.UtcNow.AddHours(1)
            });

            return RedirectToAction("Index", "Home");
        }

        // Metode for å validere passordet basert på krav
        private bool ErPassordGyldig(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool harStorBokstav = password.Any(char.IsUpper);
            bool harLitenBokstav = password.Any(char.IsLower);
            bool harTall = password.Any(char.IsDigit);

            return harStorBokstav && harLitenBokstav && harTall;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AuthCookie");
            return RedirectToAction("Index", "Home");
        }
    }
}
