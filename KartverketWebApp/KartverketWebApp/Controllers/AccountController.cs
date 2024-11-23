using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using KartverketWebApp.Data;
using KartverketWebApp.Models;
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
            IPasswordHasher<IdentityUser> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
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

            if (!ErEpostGyldig(model.Username))
            {
                ModelState.AddModelError(string.Empty, "Ugyldig e-postformat. Sørg for at e-postadressen er riktig skrevet.");
                return View(model);
            }

            var bruker = _context.Bruker.FirstOrDefault(b => b.Email == model.Username);
            if (bruker == null)
            {
                ModelState.AddModelError(string.Empty, "Feil e-postadresse eller passord.");
                return View(model);
            }

            bool isPasswordValid;
            try
            {
                // Sjekk klartekst først (for eksisterende brukere)
                isPasswordValid = bruker.Passord == model.Password;

                // Hvis klartekst ikke matcher, prøv hashed passord
                if (!isPasswordValid)
                {
                    var hashedValidationResult = _passwordHasher.VerifyHashedPassword(null, bruker.Passord, model.Password);
                    isPasswordValid = hashedValidationResult == PasswordVerificationResult.Success;
                }
            }
            catch
            {
                isPasswordValid = false;
            }

            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Feil e-postadresse eller passord.");
                return View(model);
            }

            var person = _context.Person.FirstOrDefault(p => p.BrukerId == bruker.BrukerId);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, bruker.Email),
                new Claim("Fornavn", person?.Fornavn ?? "Ukjent"),
                new Claim("BrukerType", bruker.BrukerType)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "AuthCookie");

            await HttpContext.SignInAsync("AuthCookie", new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
            });

        // Sjekk brukertype og omdiriger basert på dette
        if (bruker.BrukerType.Equals("Saksbehandler", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Saksbehandler", "Home");  // Anta at vi har en OversiktController med Index action
        }
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
            if (!IsRegistrationModelValid(model))
            {
                return View(model);
            }

            var bruker = await CreateBruker(model);
            await CreatePerson(model, bruker.BrukerId);
            await SignInNewUser(model, bruker);

            return RedirectToAction("Index", "Home");
        }

        private bool IsRegistrationModelValid(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }

            if (!ErPassordGyldig(model.Password))
            {
                ModelState.AddModelError(string.Empty, 
                    "Passordet må være minst 8 tegn langt, inneholde minst én stor bokstav, én liten bokstav,og ett tall");
                return false;
            }

            return true;
        }

        private async Task<Bruker> CreateBruker(RegisterViewModel model)
        {
            var hashedPassword = _passwordHasher.HashPassword(null, model.Password);
            
            var bruker = new Bruker
            {
                Email = model.Email,
                Passord = hashedPassword,
                BrukerType = "Standard"
            };

            _context.Bruker.Add(bruker);
            await _context.SaveChangesAsync();

            return bruker;
        }

        private async Task CreatePerson(RegisterViewModel model, int brukerId)
        {
            var person = new Person
            {
                Fornavn = model.Fornavn,
                Etternavn = model.Etternavn,
                BrukerId = brukerId
            };

            _context.Person.Add(person);
            await _context.SaveChangesAsync();
        }

        private async Task SignInNewUser(RegisterViewModel model, Bruker bruker)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, bruker.Email),
                new Claim("Fornavn", model.Fornavn),
                new Claim("BrukerType", bruker.BrukerType)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "AuthCookie");
            
            await HttpContext.SignInAsync(
                "AuthCookie", 
                new ClaimsPrincipal(claimsIdentity), 
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                }
            );
        }

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
